using System.Text;
using college_management.Constantes;
using college_management.Contextos.Interfaces;
using college_management.Dados;
using college_management.Dados.Modelos;
using college_management.Views;


namespace college_management.Contextos;


public class ContextoUsuarios : Contexto<Usuario>,
                                IContextoUsuarios
{
	public ContextoUsuarios(BaseDeDados baseDeDados,
	                        Usuario     usuarioContexto) :
		base(baseDeDados,
		     usuarioContexto) { }

	public void VerMatricula()
	{
		// TODO: Desenvolver um algoritmo para visualizar Matricula de um Aluno
		// [REQUISITO]: A visualização deve ser no formato descritivo
		// 
		// Ex.: Ver Matricula 2401123415
		//
		// Nome: Thiago
		// Matricula: 2401123415
		// Curso: Ciência da Computação
		// Período: 2

		if (UsuarioContexto.Cargo.TemPermissao(PermissoesAcesso
			                                       .PermissaoAcessoEscrita))
			// [REQUISITO]: A visualização do Gestor deve permitir a busca
			// de um Aluno em específico na base de dados
			//
			// Ex.: Ver Matricula do Aluno com Login == "thiago.santos" 
			//
			// [Ver Grade Horária]
			// Selecione um campo abaixo campo para realizar a busca
			//
			// [1] Login
			// [2] Id
			// [3] Matricula
			// 
			// Sua opção: 1 <- Opção que o usuário escolheu 
			// ...
			//
			// Digite o Login do Aluno: thiago.santos <- Nome
			// digitado pelo Gestor
			// ...
			throw new NotImplementedException();

		// [REQUISITO]: A visualização do Aluno deve ser somente
		// da Matricula vinculada a ele

		throw new NotImplementedException();
	}

	public void VerBoletim()
	{
		// TODO: Desenvolver um algoritmo para visualizar as Notas de um Aluno
		// [REQUISITO]: A visualização deve ser no formato relatório
		// 
		// Ex.: Ver Boletim do Aluno com Matricula 2401123415
		//
		// | MATERIA        | NOTA FINAL | STATUS   |
		// |----------------|------------|----------|
		// | Calculo 1      |    9.0     | Aprovado |
		// | Algebra Linear |    N/A     |   N/A    |

		if (UsuarioContexto.Cargo.TemPermissao(PermissoesAcesso
			                                       .PermissaoAcessoEscrita))
			// [REQUISITO]: A visualização do Gestor deve permitir a busca
			// de uma Aluno em específico na base de dados
			//
			// Ex.: Ver Boletim do Aluno com Login == "thiago.santos" 
			//
			// Selecione um campo abaixo campo para realizar a busca
			//
			// [1] Login
			// [2] Id
			// [3] Matricula
			// 
			// Sua opção: 1 <- Opção que o usuário escolheu 
			// ...
			//
			// Digite o Login do Aluno: thiago.santos <- Nome
			// digitado pelo Gestor
			// ...
			throw new NotImplementedException();

		// [REQUISITO]: A visualização do Aluno deve ser somente
		// da Matricula vinculada a ele

		throw new NotImplementedException();
	}

	public void VerFinanceiro()
	{
		throw new NotImplementedException();
	}

	public override async Task Cadastrar()
	{
		var temPermissao =
			UsuarioContexto
				.Cargo
				.TemPermissao(PermissoesAcesso
					              .PermissaoAcessoEscrita);

		InputView inputUsuario = new("Cadastrar Usuário");
		inputUsuario.ConstruirLayout();

		if (!temPermissao)
		{
			inputUsuario.LerEntrada("Erro",
			                        "Você não tem permissão "
			                        + "para acessar esse recurso. ");

			return;
		}

		Dictionary<string, string> cadastroUsuario
			= ObterCadastroUsuario(inputUsuario);

		if (cadastroUsuario["Confirma"] is not "S") return;

		var cargoEscolhido = BaseDeDados
		                     .cargos
		                     .ObterPorNome(cadastroUsuario
			                                   ["Cargo"]);

		if (cargoEscolhido is null)
		{
			inputUsuario.LerEntrada("Erro",
			                        "O Cargo inserido não foi "
			                        + "encontrado na base de dados."
			                        + "Pressione Enter para continuar.");

			return;
		}

		Usuario? novoUsuario = cargoEscolhido.Nome switch
		{
			CargosPadrao.CargoAlunos =>
				CriarAluno(cadastroUsuario, cargoEscolhido),
			_ => new Funcionario(cadastroUsuario["Login"],
			                     cadastroUsuario["Nome"],
			                     cargoEscolhido,
			                     cadastroUsuario["Senha"])
		};

		if (novoUsuario is null)
		{
			inputUsuario.LerEntrada("Erro",
			                        $"Não foi possível criar um novo {nameof(Usuario)}.\n"
			                        + "Tente novamente e verifique as informações. ");

			return;
		}

		var foiAdicionado
			= await BaseDeDados.usuarios.Adicionar(novoUsuario);

		var mensagemOperacao = foiAdicionado
			                       ? $"{nameof(Usuario)} cadastrado com sucesso.\n"
			                         + "Aperte qualquer tecla para retornar: "
			                       : $"Não foi possível cadastrar novo {nameof(Usuario)}.\n"
			                         + "Tente novamente e verifique as informações";

		inputUsuario.LerEntrada("Sair", mensagemOperacao);
	}

	private Dictionary<string, string> ObterCadastroUsuario(
		InputView inputUsuario)
	{
		KeyValuePair<string, string?>[] mensagensUsuario =
		[
			new("Nome", "Insira o Nome: "),
			new("Login", "Insira o Login: "),
			new("Senha", "Insira a Senha: "),
			new("Cargo", "Insira o Cargo: ")
		];

		foreach (KeyValuePair<string, string?> mensagem
		         in mensagensUsuario)
			inputUsuario.LerEntrada(mensagem.Key,
			                        mensagem.Value);

		KeyValuePair<string, string?>[] mensagensAluno =
		[
			new("Matricula", "Insira a Matrícula: "),
			new("Periodo", "Insira o Período: "),
			new("Curso", "Insira o nome do Curso: "),
			new("Modalidade", "Insira a Modalidade: ")
		];

		if (inputUsuario.ObterEntrada("Cargo")
		    is CargosPadrao.CargoAlunos)
			foreach (KeyValuePair<string, string?> mensagem
			         in mensagensAluno)
				inputUsuario.LerEntrada(mensagem.Key,
				                        mensagem.Value);

		DetalhesView detalhesView = new("Confirmar Cadastro",
		                                inputUsuario
			                                .EntradasUsuario);

		detalhesView.ConstruirLayout();

		StringBuilder mensagemConfirmacao = new();
		mensagemConfirmacao.AppendLine(detalhesView.Layout
		                                           .ToString());

		mensagemConfirmacao.AppendLine("Confirma o Cadastro?\n");
		mensagemConfirmacao.Append("[S]im\t[N]ão: ");

		inputUsuario.LerEntrada("Confirma",
		                        mensagemConfirmacao.ToString());

		return inputUsuario.EntradasUsuario;
	}

	private Aluno? CriarAluno(
		Dictionary<string, string> cadastroUsuario,
		Cargo                      cargoAlunos)
	{
		var conversaoValida
			= int.TryParse(cadastroUsuario["Matricula"],
			               out var numeroMatricula);

		if (!conversaoValida) return null;

		conversaoValida
			= int.TryParse(cadastroUsuario["Periodo"],
			               out var periodoCurso);

		if (!conversaoValida) return null;

		var cursoEscolhido = BaseDeDados
		                     .cursos
		                     .ObterPorNome(cadastroUsuario
			                                   ["Curso"]);

		if (cursoEscolhido is null) return null;

		var modalidadeCurso =
			cadastroUsuario["Modalidade"] switch
			{
				"Ead"        => Modalidade.Ead,
				"Presencial" => Modalidade.Presencial,
				"Hibrido"    => Modalidade.Hibrido,
				_            => Modalidade.Invalido
			};

		if (modalidadeCurso is Modalidade.Invalido) return null;

		Matricula novaMatricula
			= new(numeroMatricula,
			      periodoCurso,
			      cursoEscolhido,
			      modalidadeCurso);

		var novoAluno = new Aluno(cadastroUsuario["Login"],
		                          cadastroUsuario["Nome"],
		                          cargoAlunos,
		                          cadastroUsuario["Senha"],
		                          novaMatricula);

		return novoAluno;
	}

	public override async Task Editar()
	{
		throw new NotImplementedException();
	}

	public override async Task Excluir()
	{
		throw new NotImplementedException();
	}

	public override void Visualizar()
	{
		throw new NotImplementedException();
	}
}

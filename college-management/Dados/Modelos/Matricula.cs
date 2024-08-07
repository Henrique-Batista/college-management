namespace college_management.Dados.Modelos;

public sealed class Matricula : Modelo
{
    public Matricula(long numeroMatricula, int periodo, Curso curso, Modalidade modalidade)
    {
        Numero = numeroMatricula;
        Periodo = periodo;
        Curso = curso;
        Modalidade = modalidade;
    }
    
    public long Numero { get; set; }
    public int Periodo { get; set; }
    public Curso Curso { get; set; }
    public Modalidade Modalidade { get; set; }
}

public enum Modalidade
{
    Presencial, Ead, Hibrido
}
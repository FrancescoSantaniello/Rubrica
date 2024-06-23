using System.ComponentModel.DataAnnotations;
namespace Rubrica.Models;
public class Contatto
{
	[Key]
	private int _id;
	
	private string _nome;
	private string _cognome;
	private string _telefono;
	private string _userId;
	private string _altro;

	public int Id
	{
		get => _id;
		set
		{
			if (value < 0)
				throw new ArgumentException("Id non valido");
			_id = value;
		}
	}
	public string UserId
	{
		get => _userId;
		set => _userId = value;
	}
	public string Nome
	{
		get => _nome;
		set
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Nome non valido");
			
			_nome = value;
		}
	}
	public string Cognome
	{
		get => _cognome;
		set
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Cognome non valido");
			
			_cognome = value;
		}
	}
	public string Telefono
	{
		get => _telefono;
		set
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Numero di telefono non valido");

			_telefono = value;
		}
	}
	
	public string Altro
	{
		get => _altro;
		set
		{
			if (string.IsNullOrEmpty(value))
				value = "";

			_altro = value;
		}
	}

	public override string ToString()
	{
		return $"{Id};{UserId};{Nome};{Cognome};{Telefono};{Altro}";
	}
}

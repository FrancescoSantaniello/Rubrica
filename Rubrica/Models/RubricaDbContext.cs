using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Rubrica.Models;
public class RubricaDbContext : DbContext
{
	private static Lazy<RubricaDbContext> Instance = new();
	public static RubricaDbContext GetInstance(string dbPath)
	{
		if (!Instance.IsValueCreated)
			CreateInstance(dbPath);
		else if (dbPath != Percorso)
			CreateInstance(dbPath);

		return Instance.Value;
	}
	private static void CreateInstance(string dbPath)
	{
		if (string.IsNullOrEmpty(dbPath))
			throw new ArgumentException("Rubrica non valida");

		Percorso = dbPath;
		Instance = new(() => new());

		if (!File.Exists(dbPath))
			Instance.Value.Database.EnsureCreated();
	}
	public DbSet<Contatto> Contatti { get; set; }
	public static string Percorso { get; protected set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite($"Data Source={Percorso}");
	}
}
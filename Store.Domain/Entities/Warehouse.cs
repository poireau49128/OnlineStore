namespace Store.Domain.Entities;

public class Warehouse
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }

    private Warehouse() { }

    public Warehouse(string name, string? address = null)
    {
        Name = name;
        Address = address;
    }
}

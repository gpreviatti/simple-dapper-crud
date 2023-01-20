namespace Core.Entity;

public class Person
{
    public Person(string id, string name, string? email = null)
    {
        Id = id;
        Name = name;
        Email = email!;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
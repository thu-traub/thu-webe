public class PersonListMemory : IPersonConnector
{
    List<Person> plist = new List<Person>();
    public PersonListMemory()
    {
        plist.Add(new Person { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 });
        plist.Add(new Person { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 25 });
        plist.Add(new Person { Id = 3, FirstName = "Bob", Age = 40 });
    }
    public Person Create(Person person)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<Person>? Get()
    {
        return plist;
    }

    public Person? Get(int id)
    {
        return plist.FirstOrDefault(p => p.Id == id);
    }

    public bool Update(Person person)
    {
        Person? old = plist.FirstOrDefault(p => p.Id == person.Id);
        if (old != null)
        {
            old.FirstName = person.FirstName;
            old.LastName = person.LastName;
            old.Age = person.Age;
            return true;
        } else
        {
            return false;
        }
    }
}
using System.Text.Json;

public class PersonListJson : IPersonConnector
{
    const string FILEPATH = "data/personlist.json"; 
    List<Person> plist = new List<Person>();
    ILogger<PersonListJson> logger;
    public PersonListJson(ILogger<PersonListJson> logger)
    {
        this.logger = logger;
        if (File.Exists(FILEPATH))
        {
            string json = File.ReadAllText(FILEPATH);
            plist = JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
        } else
        {
            plist.Add(new Person { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 });
            plist.Add(new Person { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 25 });
            plist.Add(new Person { Id = 3, FirstName = "Bob", Age = 40 });
            Save();
        }
    }
    void Save()
    {
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(plist, options);
        File.WriteAllText(FILEPATH, json);
    }
    public Person Create(Person person)
    {
        lock(plist)
        {
            int maxid = plist.Count > 0 ? plist.Max(p => p.Id) : 0;
            person.Id = maxid + 1;
            plist.Add(person);
            Save();
            return person;
        }
    }

    public bool Delete(int id)
    {
        lock(plist)
        {
            Person? old = plist.FirstOrDefault(p => p.Id == id);
            if (old != null)        {
                plist.Remove(old);
                Save();
                return true;
            } else
            {
                return false;
            }
        }
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
            lock(plist)
            {
                logger.LogInformation($"Updating person with id {person.Id}");
                old.FirstName = person.FirstName;
                old.LastName = person.LastName;
                old.Age = person.Age;
                Save();
                logger.LogInformation($"Person with id {person.Id} updated successfully");
            }
            return true;
        } else
        {
            return false;
        }
    }
}
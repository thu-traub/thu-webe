public interface IPersonConnector
{
    List<Person>? Get();
    Person? Get(int id);
    Person Create(Person person);
    bool Update(Person person);
    bool Delete(int id);
}
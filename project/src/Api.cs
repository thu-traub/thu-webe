public static class ApiExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        const string apiBasePath = "/api/person";

        app.MapGet(apiBasePath, (IPersonConnector personConnector) =>
        {
            return personConnector.Get();
        });
        app.MapGet($"{apiBasePath}/{{id}}", (int id, IPersonConnector personConnector) =>
        {
            return personConnector.Get(id);
        });
        return app;
    }
}

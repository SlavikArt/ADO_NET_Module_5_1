using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

class Program
{
    static void Main()
    {
        // Task 3

        Console.OutputEncoding = Encoding.UTF8;

        string connectionString = ConfigurationManager.ConnectionStrings["CountriesDB"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Відобразити усі столиці, в назвах яких є літери 'a' та 'p'.
            DataTable citiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cities WHERE is_capital = 1", connection))
            {
                adapter.Fill(citiesTable);
            }

            var capitalsWithAandP = citiesTable.AsEnumerable()
                .Where(row => row.Field<string>("city_name")
                    .Contains("a", StringComparison.OrdinalIgnoreCase) 
                && row.Field<string>("city_name")
                    .Contains("p", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var capital in capitalsWithAandP)
                Console.WriteLine($"Capital Name: {capital.Field<string>("city_name")}");
            Console.WriteLine();

            // Відобразити усі столиці, в яких назва починається з літери 'k'.
            var capitalsStartingWithK = citiesTable.AsEnumerable()
                .Where(row => row.Field<string>("city_name")
                    .StartsWith("k", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var capital in capitalsStartingWithK)
                Console.WriteLine($"Capital Name: {capital.Field<string>("city_name")}");
            Console.WriteLine();

            // Відобразити назву країн, площа яких зазначена у вказаному діапазоні.
            DataTable countriesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Countries", connection))
            {
                adapter.Fill(countriesTable);
            }

            double minRange = 400000;
            double maxRange = 900000;

            var countriesInAreaRange = countriesTable.AsEnumerable()
                .Where(row => row.Field<double>("country_area") >= minRange
                && row.Field<double>("country_area") <= maxRange)
                .ToList();
            foreach (var country in countriesInAreaRange)
                Console.WriteLine($"Country Name: {country.Field<string>("country_name")}");
            Console.WriteLine();

            // Відобразити назву країн, в яких кількість жителів більша за зазначену кількість.
            long populationRange = 50000000;

            var countriesWithLargePopulation = countriesTable.AsEnumerable()
                .Where(row => row.Field<long>("country_population") > populationRange)
                .ToList();
            foreach (var country in countriesWithLargePopulation)
                Console.WriteLine($"Country Name: {country.Field<string>("country_name")}");
            Console.WriteLine();
        }
    }
}

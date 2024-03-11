using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

class Program
{
    static void Main()
    {
        // Task 2

        Console.OutputEncoding = Encoding.UTF8;

        string connectionString = ConfigurationManager.ConnectionStrings["CountriesDB"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Відобразити всю інформацію про країни.
            DataTable countriesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Countries", connection))
            {
                adapter.Fill(countriesTable);
            }

            var allCountries = countriesTable.AsEnumerable();
            foreach (var country in allCountries)
                Console.WriteLine(
                    $"Id: {country.Field<int>("id_country"),-4}" +
                    $"Name: {country.Field<string>("country_name"),-11}" +
                    $"Population: {country.Field<long>("country_population"),-12}" +
                    $"Area: {country.Field<double>("country_area"),-10}" +
                    $"Is in EU: {country.Field<bool>("is_in_eu"),-7}" +
                    $"Continent Id: {country.Field<int>("id_continent"),-10}"
                );
            Console.WriteLine();

            // Відобразити назву країн.
            var countryNames = allCountries
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in countryNames)
                Console.WriteLine($"Country Name: {name}");
            Console.WriteLine();

            // Відобразити назву столиць.
            DataTable citiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cities WHERE is_capital = 1", connection))
            {
                adapter.Fill(citiesTable);
            }

            var capitalNames = citiesTable.AsEnumerable()
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in capitalNames)
                Console.WriteLine($"Capital Name: {name}");
            Console.WriteLine();

            // Відобразити назву великих міст певної країни.
            string countryName = "Ukraine";

            DataTable bigCitiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Cities WHERE city_population > 100000 AND id_region IN (SELECT id_region FROM Regions WHERE id_country IN (SELECT id_country FROM Countries WHERE country_name = '{countryName}'))", connection))
            {
                adapter.Fill(bigCitiesTable);
            }

            var bigCitiesOfCountry = bigCitiesTable.AsEnumerable()
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in bigCitiesOfCountry)
                Console.WriteLine($"Big City Name: {name}");
            Console.WriteLine();

            // Відобразити назву столиць з кількістю жителів понад п'ять мільйонів.
            var bigCapitals = citiesTable.AsEnumerable()
                .Where(row => row.Field<long>("city_population") > 5000000 
                && row.Field<bool>("is_capital"))
                .Select(row => row.Field<string>("city_name"))
                .ToList();
            foreach (var name in bigCapitals)
                Console.WriteLine($"Big Capital Name: {name}");
            Console.WriteLine();

            // Відобразити назву усіх європейських країн.
            var europeanCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in europeanCountries)
                Console.WriteLine($"European Country Name: {name}");
            Console.WriteLine();

            // Відобразити назву країн з площею, більшою від певного числа.
            double areaRange = 100000;

            var largeCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<double>("country_area") > areaRange)
                .Select(row => row.Field<string>("country_name"))
                .ToList();
            foreach (var name in largeCountries)
                Console.WriteLine($"Large Country Name: {name}");
            Console.WriteLine();
        }
    }
}

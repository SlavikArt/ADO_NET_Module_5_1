using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

class Program
{
    static void Main()
    {
        // Task 4

        Console.OutputEncoding = Encoding.UTF8;

        string connectionString = ConfigurationManager.ConnectionStrings["CountriesDB"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Показати Топ-5 країни за площею.
            DataTable countriesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Countries", connection))
            {
                adapter.Fill(countriesTable);
            }

            var top5CountriesByArea = countriesTable.AsEnumerable()
                .OrderByDescending(row => row.Field<double>("country_area"))
                .Take(5)
                .ToList();
            foreach (var country in top5CountriesByArea)
                Console.WriteLine(
                    $"Country Name: {country.Field<string>("country_name"),-11}" +
                    $"Area: {country.Field<double>("country_area")}"
                );
            Console.WriteLine();

            // Показати Топ-5 столиці за кількістю жителів.
            DataTable citiesTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Cities WHERE is_capital = 1", connection))
            {
                adapter.Fill(citiesTable);
            }

            var top5CapitalsByPopulation = citiesTable.AsEnumerable()
                .OrderByDescending(row => row.Field<long>("city_population"))
                .Take(5)
                .ToList();
            foreach (var capital in top5CapitalsByPopulation)
                Console.WriteLine(
                    $"Capital Name: {capital.Field<string>("city_name"),-8}" +
                    $"Population: {capital.Field<long>("city_population")}"
                );
            Console.WriteLine();

            // Показати країну з найбільшою площею.
            var countryWithLargestArea = countriesTable.AsEnumerable()
                .OrderByDescending(row => row.Field<double>("country_area"))
                .First();
            Console.WriteLine(
                $"Country with Largest Area: {countryWithLargestArea.Field<string>("country_name"),-8}" +
                $"Area: {countryWithLargestArea.Field<double>("country_area")}\n"
            );

            // Показати столицю з найбільшою кількістю жителів.
            var capitalWithLargestPopulation = citiesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_capital"))
                .OrderByDescending(row => row.Field<long>("city_population"))
                .First();
            Console.WriteLine(
                $"Capital with Largest Population: {capitalWithLargestPopulation.Field<string>("city_name"),-8}" +
                $"Population: {capitalWithLargestPopulation.Field<long>("city_population")}\n"
            );

            // Показати країну з найменшою площею в Європі.
            var smallestEuropeanCountry = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .OrderBy(row => row.Field<double>("country_area"))
                .First();
            Console.WriteLine(
                $"Smallest European Country: {smallestEuropeanCountry.Field<string>("country_name"),-9}" +
                $"Area: {smallestEuropeanCountry.Field<double>("country_area")}\n"
            );

            // Показати середню площу країн в Європі.
            var averageAreaOfEuropeanCountries = countriesTable.AsEnumerable()
                .Where(row => row.Field<bool>("is_in_eu"))
                .Average(row => row.Field<double>("country_area"));
            Console.WriteLine(
                $"Average Area of European Countries: {Math.Round(averageAreaOfEuropeanCountries,2)}\n"
            );

            // Показати Топ-3 міст за кількістю мешканців для певної країни.
            string countryName = "Ukraine";

            DataTable citiesOfCountryTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Cities WHERE id_region IN (SELECT id_region FROM Regions WHERE id_country IN (SELECT id_country FROM Countries WHERE country_name = '{countryName}'))", connection))
            {
                adapter.Fill(citiesOfCountryTable);
            }

            var top3CitiesOfCountry = citiesOfCountryTable.AsEnumerable()
                .OrderByDescending(row => row.Field<long>("city_population"))
                .Take(3)
                .ToList();
            foreach (var city in top3CitiesOfCountry)
                Console.WriteLine($"City Name: {city.Field<string>("city_name"),-6}" +
                    $"Population: {city.Field<long>("city_population")}\n"
                );

            // Показати загальну кількість країн.
            var totalNumberOfCountries = countriesTable.AsEnumerable()
                .Count();
            Console.WriteLine(
                $"Total Number of Countries: {totalNumberOfCountries}\n"
            );

            // Показати частину світу з максимальною кількістю країн.
            DataTable continentsTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Continents", connection))
            {
                adapter.Fill(continentsTable);
            }

            var continentWithMostCountries = continentsTable.AsEnumerable()
                .OrderByDescending(row => countriesTable.AsEnumerable()
                    .Count(c => c.Field<int>("id_continent") == row.Field<int>("id_continent")))
                .First();
            Console.WriteLine(
                $"Continent with Most Countries: {continentWithMostCountries.Field<string>("continent_name")}\n"
            );

            // Показати кількість країн у кожній частині світу.
            foreach (var continent in continentsTable.AsEnumerable())
            {
                var numOfCountriesInContinent = countriesTable.AsEnumerable()
                    .Count(c => c.Field<int>("id_continent") == continent.Field<int>("id_continent"));
                Console.WriteLine(
                    $"Continent: {continent.Field<string>("continent_name"),-15}" +
                    $"Number of Countries: {numOfCountriesInContinent}"
                );
            }
            Console.WriteLine();
        }
    }
}

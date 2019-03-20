using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PalotaInterviewCS
{
    /// Editor:  Kgantsi Rabatseta (kgantsi.rabatseta@gmail.com,kgantsi@blaqcode.co.za) c(0736682915) 
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string countriesEndpoint = "https://restcountries.eu/rest/v2/all";

        static void Main(string[] args)
        {
           
            var countriesList  = GetCountries(countriesEndpoint).GetAwaiter().GetResult().FromJsonString<List<Country>>();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Palota Interview: Country Facts");
            Console.WriteLine();
            Console.ResetColor();
            if (countriesList == null || countriesList.Count == 0)
            {
                Console.WriteLine("No Countries to display");
                return;
            }
            //var rnd = new Random(); // random to populate fake answer - you can remove this once you use real values

            //TODO use data operations and data structures to optimally find the correct value (N.B. be aware of null values)

            /*
             * HINT: Sort the list in descending order to find South Africa's place in terms of gini coefficients
             * `Country.Gini` is the relevant field to use here           
             */
            countriesList = countriesList.OrderByDescending(c => c.Gini).ToList();//rnd.Next(1, 10); // Use correct value
            int southAfricanGiniPlace = (countriesList.IndexOf(countriesList.FirstOrDefault(c=>c.Name.Equals("south africa",StringComparison.InvariantCultureIgnoreCase))))+1;
            Console.WriteLine($"1. South Africa's Gini coefficient is the {GetOrdinal(southAfricanGiniPlace)} highest");

            /*
             * HINT: Sort the list in ascending order or just find the Country with the minimum gini coeficient          
             * use `Country.Gini` for the ordering then return the relevant country's name `Country.Name`
             */
            string lowestGiniCountry = countriesList.Last().Name; // Use correct value
            Console.WriteLine($"2. {lowestGiniCountry} has the lowest Gini Coefficient");

            /*
             * HINT: Group by regions (`Country.Region`), then count the number of unique timezones that the countries in each region span
             * Once you have done the grouping, find the group `Region` with the most timezones and return it's name and the number of unique timezones found          
             */
            var regionsGroup = countriesList.GroupBy(c => c.Region)
                .Select(r => new { region = r.Key, timeZones = r.Where(t=>t.Timezones!=null).SelectMany(rr=>rr.Timezones).Distinct().Count()})
                .OrderBy(r=>r.timeZones)
                .Last();
            string regionWithMostTimezones = regionsGroup.region.ToString(); // Use correct value
            int amountOfTimezonesInRegion = regionsGroup.timeZones;
            Console.WriteLine($"3. {regionWithMostTimezones} is the region that spans most timezones at {amountOfTimezonesInRegion} timezones");

            /*
             * HINT: Count occurances of each currency in all countries (check `Country.Currencies`)
             * Find the name of the currency with most occurances and return it's name (`Currency.Name`) also return the number of occurances found for that currency          
             */
            var mostPopularCurrencyObj = countriesList.SelectMany(cl => (cl.Currencies == null ? new Currency[0] : cl.Currencies))
               .GroupBy(c => c.Name)
               .Select(c => new { name = c.Key, occurances = c.Count() })
               .OrderBy(c => c.occurances)
               .Last();
            string mostPopularCurrency = mostPopularCurrencyObj.name; ; // Use correct value
            int numCountriesUsedByCurrency = mostPopularCurrencyObj.occurances; // Use correct value
            Console.WriteLine($"4. {mostPopularCurrency} is the most popular currency and is used in {numCountriesUsedByCurrency} countries");

            /*
             * HINT: Count the number of occurances of each language (`Country.Languages`) and sort then in descending occurances count (i.e. most populat first)
             * Once done return the names of the top three languages (`Language.Name`)
             */
            string[] mostPopularLanguages = countriesList.SelectMany(c=>(c.Languages==null?new Language[0]:c.Languages))
                .GroupBy(l=>new { l.Iso6391, l.Name })
                .Select(l=>new { l.Key.Name,count =l.Count()})
                .OrderByDescending(l=>l.count)
                .Select(c=>c.Name)
                .Take(3)
                .ToArray();
            // Use correct values
            if(mostPopularLanguages.Count()==3)
            Console.WriteLine($"5. The top three popular languages are {mostPopularLanguages[0]}, {mostPopularLanguages[1]} and {mostPopularLanguages[2]}");

            /*
             * HINT: Each country has an array of Bordering countries `Country.Borders`, The array has a list of alpha3 codes of each bordering country 
             * `Country.alpha3Code`
             * Sum up the population of each country (`Country.Population`) along with all of its bordering countries's population.
             * Sort this list according to the combined population descending
             * Find the country with the highest combined (with bordering countries) population the return that country's name (`Country.Name`), 
             * the number of it's Bordering countries (`Country.Borders.length`) 
             * and the combined population
             * Be wary of null values           
             */
            var countryOfFocus = countriesList.Select(c =>
            new
            {
                c.Name,
                borderCount = (c.Borders == null ? 0 : c.Borders.Count()),
                combinedPopulation = (c.Borders == null ? 0 : countriesList.Where(cb =>
                    c.Borders.Any(b =>b == cb.Alpha3Code)).Sum(cS => cS.Population)
                   ) + c.Population
            }).OrderByDescending(c => c.combinedPopulation).First();

            string countryWithBorderingCountries = countryOfFocus.Name; // Use correct value
            int numberOfBorderingCountries = countryOfFocus.borderCount; // Use correct value
            long combinedPopulation = countryOfFocus.combinedPopulation; // Use correct value
            Console.WriteLine($"6. {countryWithBorderingCountries} and it's {numberOfBorderingCountries} bordering countries has the highest combined population of {combinedPopulation}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the lowest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of null values when doing calculations           
             
             */
            var populationDenseCountries = countriesList.Select(c => new { c.Name, density = ((double)c.Population) / (c.Area.HasValue?c.Area.Value:1D) }).OrderBy(c => c.density);
            string lowPopDensityName = populationDenseCountries.First().Name; // Use correct value
            double lowPopDensity = populationDenseCountries.First().density; // Use correct value
            Console.WriteLine($"7. {lowPopDensityName} has the lowest population density of {lowPopDensity}");

            /*
             * HINT: Population density is calculated as (population size)/area, i.e. `Country.Population/Country.Area`
             * Calculate the population density of each country and sort by that value to find the highest density
             * Return the name of that country (`Country.Name`) and its calculated density.
             * Be wary of any null values when doing calculations. Consider reusing work from above related question           
             */
            string highPopDensityName = populationDenseCountries.Last().Name; // Use correct value
            double highPopDensity = populationDenseCountries.Last().density; // Use correct value
            Console.WriteLine($"8. {highPopDensityName} has the highest population density of {highPopDensity}");

            /*
             * HINT: Group by subregion `Country.Subregion` and sum up the area (`Country.Area`) of all countries per subregion
             * Sort the subregions by the combined area to find the maximum (or just find the maximum)
             * Return the name of the subregion
             * Be wary of any null values when summing up the area           
             */
            var subregion = countriesList.GroupBy(c => c.Subregion)
               .Select(c => new { name = c.Key, area = c.Sum(ca => (ca.Area.HasValue ? ca.Area.Value : 0D)) })
               .OrderBy(c => c.area)
               .First();
            string largestAreaSubregion = subregion.name; // Use correct value
            Console.WriteLine($"9. {largestAreaSubregion} is the subregion that covers the most area");

            /*
             * HINT: Group by regional blocks (`Country.RegionalBlocks`). For each regional block, average out the gini coefficient (`Country.Gini`)
             * of all member countries
             * Sort the regional blocks by the average country gini coefficient to find the lowest (or find the lowest without sorting)
             * Return the name of the regional block (`RegionalBloc.Name`) along with the calculated average gini coefficient
             */
            //This Question is somewhat confusing in terms of what it exactly

            var regionBlocksList = countriesList.SelectMany(c => c.RegionalBlocs)
                .Select(r=> new {r.Name,r.Acronym })
                .Distinct()
                .ToList();

            var blockCountries = regionBlocksList.Select(r => new
            {
                name = r.Name,
                gini = countriesList.Where(c =>
                    c.RegionalBlocs != null &&
                    c.RegionalBlocs.Any(rb => rb.Name.Equals(r.Name))).Select(c=>new { c.Gini}).Average(a => (a.Gini.HasValue ? a.Gini.Value : 0D))
            })
            .OrderBy(g => g.gini).ToList();
            
            string mostEqualRegionalBlock = blockCountries.First().name; 
            double lowestRegionalBlockGini = blockCountries.First().gini;
            Console.WriteLine($"10. {mostEqualRegionalBlock} is the regional block with the lowest average Gini coefficient of {lowestRegionalBlockGini}");
        }

        /// <summary>
        /// Gets the countries from a specified endpiny
        /// </summary>
        /// <returns>The countries.</returns>
        /// <param name="path">Path endpoint for the API.</param>
        static async Task<string> GetCountries(string path)
        {
            string countries = null;
            try
            {
                //TODO get data from endpoint and convert it to a typed array using Country.FromJson
                HttpResponseMessage response =await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    countries = await response.Content.ReadAsStringAsync();
                }
            }
            catch(Exception e)
            {
                //Kgantsi:  We would probably log the exception here but since there is no logger ill ignore the exception declaration
                countries = null;
            }
            return countries;
        }

        /// <summary>
        /// Gets the ordinal value of a number (e.g. 1 to 1st)
        /// </summary>
        /// <returns>The ordinal.</returns>
        /// <param name="num">Number.</param>
        public static string GetOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }

        }
    }

    public static class Helper
    {
        public static T FromJsonString<T>(this string json)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }
    }
}

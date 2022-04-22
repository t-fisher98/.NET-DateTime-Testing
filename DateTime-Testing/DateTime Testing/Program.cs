const double Buffer = 20;

TimeSpan earliestStartTime = new TimeSpan(11, 0, 0);
TimeSpan latestEndTime = new TimeSpan(23, 0, 0);
TimeSpan totalRunTime = new();
TimeSpan movieEndTime = new();

List<Movie> movies = new();
List<MovieBooking> movieBookings = new();
List<ShowTime> bookedMovies = new();

List<Exception> errorList = new();

PopulateShowTimes();
PopulateMovies();

foreach (MovieBooking booking in movieBookings)
{
    if (booking.MovieID == 0)
        errorList.Add(new Exception("Movie ID is required."));

    Movie? movieExists = movies.Where(m => m.MovieID == booking.MovieID).FirstOrDefault();

    if (movieExists == null)
        errorList.Add(new Exception($"{movieExists.Title} does not exist."));

    if (booking.StartTime.TimeOfDay < earliestStartTime)
        errorList.Add(new Exception($"{movieExists.Title} will start too early. The start time must be later than 11:00am."));

    totalRunTime = TimeSpan.FromMinutes((double)movieExists.Length + Buffer);

    if(booking.StartTime.TimeOfDay < movieEndTime)
        errorList.Add(new Exception($"{movieExists.Title} will start before the previous movie ends. Please schedule this movie after {movieEndTime}"));

    movieEndTime = booking.StartTime.TimeOfDay + totalRunTime;

    if (movieEndTime > latestEndTime)
        errorList.Add(new Exception($"{movieExists.Title} will end after 11:00 PM. No movie can end past this time."));

    bookedMovies.Add(new ShowTime(booking.MovieID, movieExists.Title, booking.StartTime, movieEndTime));
}

if(errorList.Count > 0)
{
    foreach(var error in errorList)
    {
        Console.WriteLine(error.ToString());
    }
}
else
{
    foreach(ShowTime showTime in bookedMovies)
    {
        Console.WriteLine(showTime.ToString());
    }
}

void PopulateMovies()
{
    movies.Add(new Movie(1, "Avengers", 120));
    movies.Add(new Movie(2, "Thor", 85));
    movies.Add(new Movie(3, "Loki", 95));
    movies.Add(new Movie(4, "Spider-Man", 130));
    movies.Add(new Movie(5, "Iron Man", 150));
}

void PopulateShowTimes()
{
    movieBookings.Add(new MovieBooking(2, new DateTime(2022, 04, 22, 11, 30, 00)));
    movieBookings.Add(new MovieBooking(5, new DateTime(2022, 04, 22, 13, 20, 00)));
    movieBookings.Add(new MovieBooking(4, new DateTime(2022, 04, 22, 16, 10, 00)));
    movieBookings.Add(new MovieBooking(1, new DateTime(2022, 04, 22, 18, 40, 00)));
    movieBookings.Add(new MovieBooking(3, new DateTime(2022, 04, 22, 21, 00, 00)));
}

public class Movie
{
    public int MovieID { get; set; }
    public string Title { get; set; }
    public int Length { get; set; }

    public Movie(int movieID, string title, int length)
    {
        MovieID = movieID;
        Title = title;
        Length = length;
    }
}

public class MovieBooking
{
    public int MovieID { get; set; }
    public DateTime StartTime { get; set; }

    public MovieBooking(int movieID, DateTime startTime)
    {
        MovieID = movieID;
        StartTime = startTime;
    }
}

public class ShowTime
{
    public int MovieID { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public ShowTime(int movieID, string title, DateTime startTime, TimeSpan endTime)
    {
        MovieID = movieID;
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
    }

    public override string ToString()
    {
        return $"MovieID: {MovieID}\n Title: {Title}\n StartTime: {StartTime.TimeOfDay}\n EndTime: {EndTime}\n";
    }
}
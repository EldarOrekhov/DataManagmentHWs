namespace GameStudioClasses
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Developer { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }

        public override string ToString()
        {
            return $"Game(Id={Id}, Name={Name}, Developer={Developer}, Genre={Genre}, ReleaseDate={ReleaseDate.ToShortDateString()})";
        }
    }
}

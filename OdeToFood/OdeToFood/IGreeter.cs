namespace OdeToFood
{
    public interface IGreeter
    {
        string GetMessageOfTheDay();
    }

    public class Greeter : IGreeter
    {
        string IGreeter.GetMessageOfTheDay()
        {
            return "Greetings from the Greeter interface";
        }
    }
}
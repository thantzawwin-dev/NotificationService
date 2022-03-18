namespace NotificationService;

public class Helper {

  	public static string GenerateRandomDigit(int count, int minimumRange = 0, int maximumRange = 9)
    {
        string result = String.Empty;
        Random numberGen = new Random();

        for(int i = 0; i < count; i++)
        {
            int randomNumber = numberGen.Next(minimumRange, maximumRange);
            result += String.Join(result, randomNumber.ToString());
        }
        return result;
    }

}

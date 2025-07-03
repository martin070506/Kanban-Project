using BackendTests.Tests;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace BackendTests
{
    internal class TestRunner
    {
        public static void Main(string[] args)
        {
            AbstractTestRunner[] test = new AbstractTestRunner[]
            {
                //new TestUserService(),
                //new TestBoardService(),
                //new TestTaskService(),
                //new TestUserBoardsService(),
                new TestGradingService()
            };

            foreach (AbstractTestRunner t in test)
            {
                try
                {
                    t.RunAllTests();
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        $"An error occurred while running tests of {t.GetType()} : {e.Message}"
                    );
                }
            }
        }
    }
}

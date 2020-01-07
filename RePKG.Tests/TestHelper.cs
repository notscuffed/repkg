using System;

namespace RePKG.Tests
{
    public static class TestHelper
    {
        static TestHelper()
        {
            BasePath = AppContext.BaseDirectory.Split(
                           new[] {"RePKG.Tests"}, 
                           StringSplitOptions.RemoveEmptyEntries)[0] + "RePKG.Tests";
        }
        
        public static string BasePath { get; }
    }
}
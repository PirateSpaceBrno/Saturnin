using Saturnin.Helpers;
using System.Linq;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace Saturnin.Tests.Steps
{
    [Binding]
    public class TextHelperSteps
    {
        [When(@"I convert '(.*)' to ASCII")]
        public void ConvertRemoveDiacritics(string text)
        {
            ScenarioContext.Current.Add("ConvertRemoveDiacritics", text.RemoveDiacritics());
        }

        [Then(@"Converted text is '(.*)'")]
        public void ConvertRemoveDiacriticsResult(string expectedText)
        {
            Assert.AreEqual(expectedText, ScenarioContext.Current.FirstOrDefault(x => x.Key == "ConvertRemoveDiacritics").Value);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebLibrary;
using Parsers;
using System.Text.RegularExpressions;

namespace Task5.UnitTests
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]

        public void ConsoleWriterTest()
        {
            ConsoleWriter cs = new ConsoleWriter();
            try
            {
                ConsoleWriter.WtireLines(null);
                ConsoleWriter.WtireLines(new List<string>() { "string1" , null});
                ConsoleWriter.WtireLines(new string[1] { ((int)ErrorType.NotAcceptable).ToString() + ".Service"});
                ConsoleWriter.WtireLines(new string[] {"string1","string2"});
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            Assert.Pass();
        }
        [Test]
        public void LinksTest()
        {
            var services = new List<JSONParser> { new TomorrowIOParser(), new OpenWeatherParser(), new StormGlassParser() };

            foreach (var service in services)
            {
                var data = new Data(service.GetType());
                var link = data.Link;
                if (!link.Contains("https://api.")) Assert.Fail();
                var header = data.Headers;
            }

            Assert.Pass();
        }

         [Test]
        public void GetRequestTets()
        {

            string[] links = new string[] { "https://www.amazon.com/", "https://key-seo.com/404", "http://hwproj.me/courses/65", null };
            List<string>[] headers = new List<string>[] { new List<string>() { "key:header" }, null  };

            foreach (string link in links)
            {
                foreach (var header in headers)
                {
                    GetRequest gr = new GetRequest(link, header);
                    string statement = gr.Send();
                    switch (link)
                    {
                        case "https://www.amazon.com/":
                            {
                                if (statement != "AllFine" || gr.GetResponce() == null)
                                {
                                    Assert.Fail();
                                }
                                break;
                            }
                        case "https://key-seo.com/404":
                            {
                                if (statement.Contains(ErrorType.NotFound.ToString()))
                                {
                                    Assert.Fail();
                                }
                                break;
                            }
                        case "http://hwproj.me/courses/65":
                            {
                                if (statement.Contains(ErrorType.BadGateway.ToString()))
                                {
                                    Assert.Fail();
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                
            }
            Assert.Pass();
        }

    }
}

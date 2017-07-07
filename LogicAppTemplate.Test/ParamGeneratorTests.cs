﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LogicAppTemplate.Test
{
    [TestClass]
    public class ParamGeneratorTests
    {
        [TestMethod]
        public void CreateParamGeneratorClass()
        {
            new ParamGenerator();
        }

        [TestMethod]
        public void GenerateParameterFileFromTemplate()
        {
            var content = GetEmbededFileContent("LogicAppTemplate.Test.TestFiles.paramGeneratorLogicAppTemplate.json");
            var generator = new ParamGenerator();

            var defintion = generator.CreateParameterFileFromTemplate(JObject.Parse(content));

            //check parameters
            Assert.IsNull(defintion["parameters"]["INT0014-NewHires-ResourceGroup"]);
            Assert.AreEqual("[resourceGroup().location]", defintion["parameters"]["logicAppLocation"]["value"]);
            Assert.AreEqual("INT0014-NewHires-Trigger", defintion["parameters"]["logicAppName"]["value"]);
        }

        [TestMethod]
        public void GenerateParameterFileSecureStringNoKeyVault()
        {
            var content = GetEmbededFileContent("LogicAppTemplate.Test.TestFiles.paramGenerator-securestring.json");
            var generator = new ParamGenerator();

            var defintion = generator.CreateParameterFileFromTemplate(JObject.Parse(content));

            //check parameters
            Assert.AreEqual("SQLAzure", defintion["parameters"]["logicAppName"]["value"]);
            Assert.AreEqual("[resourceGroup().location]", defintion["parameters"]["logicAppLocation"]["value"]);
            Assert.AreEqual("sql-1", defintion["parameters"]["sql-1_name"]["value"]);
            Assert.AreEqual("SQL Azure", defintion["parameters"]["sql-1_displayName"]["value"]);
            Assert.AreEqual("dummyserverone.database.windows.net", defintion["parameters"]["sql-1_server"]["value"]);
            Assert.AreEqual("dummydatabase", defintion["parameters"]["sql-1_database"]["value"]);
            Assert.AreEqual("",defintion["parameters"]["sql-1_username"]["value"]);
            Assert.AreEqual("",defintion["parameters"]["sql-1_password"]["value"]);         
        }

        [TestMethod]
        public void GenerateParameterFileSecureStringWithKeyVault()
        {
            var content = GetEmbededFileContent("LogicAppTemplate.Test.TestFiles.paramGenerator-securestring.json");
            var generator = new ParamGenerator();
            generator.KeyVault = ParamGenerator.KeyVaultUsage.Static;
            var defintion = generator.CreateParameterFileFromTemplate(JObject.Parse(content));

            //check parameters
            Assert.AreEqual("SQLAzure", defintion["parameters"]["logicAppName"]["value"]);
            Assert.AreEqual("[resourceGroup().location]", defintion["parameters"]["logicAppLocation"]["value"]);
            Assert.AreEqual("sql-1", defintion["parameters"]["sql-1_name"]["value"]);
            Assert.AreEqual("SQL Azure", defintion["parameters"]["sql-1_displayName"]["value"]);
            Assert.AreEqual("dummyserverone.database.windows.net", defintion["parameters"]["sql-1_server"]["value"]);
            Assert.AreEqual("dummydatabase", defintion["parameters"]["sql-1_database"]["value"]);
            Assert.IsNull(defintion["parameters"]["sql-1_username"]["value"]);
            Assert.AreEqual("/subscriptions/{subscriptionid}/resourceGroups/{resourcegroupname}/providers/Microsoft.KeyVault/vaults/{vault-name}", defintion["parameters"]["sql-1_username"]["reference"]["keyVault"]["id"]);
            Assert.AreEqual("sql-1_username", defintion["parameters"]["sql-1_username"]["reference"]["secretName"]);
            Assert.IsNull(defintion["parameters"]["sql-1_password"]["value"]);
            Assert.AreEqual("/subscriptions/{subscriptionid}/resourceGroups/{resourcegroupname}/providers/Microsoft.KeyVault/vaults/{vault-name}", defintion["parameters"]["sql-1_password"]["reference"]["keyVault"]["id"]);
            Assert.AreEqual("sql-1_password", defintion["parameters"]["sql-1_password"]["reference"]["secretName"]);
        }





        //var resourceName = "LogicAppTemplate.Templates.starterTemplate.json";
        private static string GetEmbededFileContent(string resourceName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();


            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }

        }
    }
}
﻿using umbraco.BusinessLogic;
using NUnit.Framework;
using System;
using umbraco.cms.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.businesslogic.web;
using System.Linq;

using System.Xml;
using System.Web;
using System.Web.Caching;

namespace umbraco.Test
{
    
    
    /// <summary>
    ///This is a test class for PermissionTest and is intended
    ///to contain all PermissionTest Unit Tests
    ///</summary>
    [TestFixture]
    public class PermissionTest
    {
        
        
		[TestFixtureSetUp]
		public void InitTestFixture()
		{
			SetUpUtilities.InitConfigurationManager();
			m_User = new User(0);
			SetUpUtilities.InitAppDomainDynamicBase();

		}

		/// <summary>
        /// Create a new permission and delete it
        /// </summary>
        [Test]
        public void Permission_Make_New()
        {
            var doc = Document.GetRootDocuments().First();            
            Permission.MakeNew(m_User, doc, ActionNew.Instance.Letter);

            //get the notifications
            var n = Permission.GetUserPermissions(m_User);
            Assert.IsTrue(n.Count() > 0);
            Assert.AreEqual(1, n.Where(x => x.NodeId == doc.Id && x.UserId == m_User.Id && x.PermissionId == ActionNew.Instance.Letter).Count());

            //delete the notification
            Permission.DeletePermissions(doc);

            //make sure they're gone
            Assert.AreEqual(0, Permission.GetNodePermissions(doc).Count());


        }

        /// <summary>
        /// Creates a new document, assigns some Permissions to it, then deletes the document.
        /// Need to ensure that the Permissions are removed.
        /// </summary>
        [Test]
        public void Permission_Assign_To_New_Node_Then_Delete_Node()
        {
            //create anew document
            var dt = DocumentType.GetAllAsList().First();
            var doc = Document.MakeNew(Guid.NewGuid().ToString("N"), dt, m_User, -1);

            //assign a Permission to it
            Permission.MakeNew(m_User, doc, ActionNew.Instance.Letter);

            //delete the document permanently
            doc.delete(true);

            //make sure they're gone
            Assert.AreEqual(0, Permission.GetNodePermissions(doc).Count());
            Assert.IsFalse(Document.IsNode(doc.Id));

        }

        /// <summary>
        /// Create a new user, assign a Permission to it, remove the user and ensure the Permission are gone as well.
        /// </summary>
        [Test]
        public void Permission_Assign_To_New_User_Then_Delete_User()
        {
            //create anew document
            var ut = UserType.GetAllUserTypes().First();
            var u = User.MakeNew(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), ut);
            //get a doc
            var doc = Document.GetRootDocuments().First();

            //assign a notification to the user
            Permission.MakeNew(u, doc, ActionNew.Instance.Letter);

            //delete the document permanently
            u.delete();

            //make sure they're gone
            Assert.AreEqual(0, Permission.GetUserPermissions(u).Count());
            Assert.IsNull(User.GetUser(u.Id));

        }

        private User m_User;

        #region Tests to write

        ///// <summary>
        /////A test for DeletePermissions
        /////</summary>
        //[Test]
        //public void DeletePermissionsTest()
        //{
        //    CMSNode node = null; // TODO: Initialize to an appropriate value
        //    Permission.DeletePermissions(node);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DeletePermissions
        /////</summary>
        //[Test]
        //public void DeletePermissionsTest1()
        //{
        //    User user = null; // TODO: Initialize to an appropriate value
        //    CMSNode node = null; // TODO: Initialize to an appropriate value
        //    Permission.DeletePermissions(user, node);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DeletePermissions
        /////</summary>
        //[Test]
        //public void DeletePermissionsTest2()
        //{
        //    User user = null; // TODO: Initialize to an appropriate value
        //    Permission.DeletePermissions(user);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

       

        ///// <summary>
        /////A test for UpdateCruds
        /////</summary>
        //[Test]
        //public void UpdateCrudsTest()
        //{
        //    User user = null; // TODO: Initialize to an appropriate value
        //    CMSNode node = null; // TODO: Initialize to an appropriate value
        //    string permissions = string.Empty; // TODO: Initialize to an appropriate value
        //    Permission.UpdateCruds(user, node, permissions);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //} 
        #endregion

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use TestFixtureSetUp to run code before running the first test in the class
        //[TestFixtureSetUp]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use TestFixtureTearDown to run code after all tests in a class have run
        //[TestFixtureTearDown]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use SetUp to run code before running each test
        //[SetUp]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TearDown]
        //public void MyTestCleanup()
        //{
        //}
        //

        [SetUp]
        public void MyTestInitialize()
        {
			SetUpUtilities.AddUmbracoConfigFileToHttpCache();

        }
        
        /// <summary>
        /// Remove the created document type
        /// </summary>

        
		[TearDown]
        public void MyTestCleanup()
        {
			SetUpUtilities.RemoveUmbracoConfigFileFromHttpCache();
        }

        #endregion
    }
}

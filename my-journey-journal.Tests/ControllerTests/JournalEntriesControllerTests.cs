using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_journey_journal.Controllers;
using my_journey_journal.Data;
using my_journey_journal.Models;

namespace my_journey_journal.Tests.ControllerTests
{
    public class JournalEntriesControllerTests
    {
        //to simulate the DB
        private ApplicationDbContext _context;
        //to test the controller with the DB dependency
        private JournalEntriesController _controller;
        public JournalEntriesControllerTests()
        {
            // Creating mock for all dependencies in the controller.
            //_context = A.Fake<ApplicationDbContext>();
            //removed b/c turns out EF Core methods cant be mocked easily
            //instead, use EF Core's In-Memory Provider. More like 
            //an integration test just using a fast disposable db in memory

            //moved below to allow unique db for each test
            // to avoid altering the DB the more tests are added.

        }

        // Initialize DB and controller with unique DB name
        public void Initialize(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName) // Use a unique name per test class
                .Options;

            _context = new ApplicationDbContext(options);
            //SUT - unlike MVC programming, need to bring in controller.
            _controller = new JournalEntriesController(_context);

            // Optional: Seed the in-memory DB with test data
            _context.JournalEntry.Add(new JournalEntry { EntryName = "Test Entry", EntryDetails = "Details", DateCreated = DateTime.Now });
            _context.JournalEntry.Add(new JournalEntry { EntryName = "Test Entry 2", EntryDetails = "Details 2", DateCreated = DateTime.Now });
            _context.SaveChanges();
        }

        [Fact]
        public async Task JournalEntriesController_Index_ReturnsViewResultWithAllEntries()
        {
            //Arrange - what do I need to bring it in?
            // Use a unique DB name per test
            Initialize(Guid.NewGuid().ToString());

            //Act
            var result = await _controller.Index();

            //Assert
            // while the def for index says IActionResult,
            // it returns a ViewResult, a subtype. keeps it flexible.
            result.Should().BeOfType<ViewResult>();
            result.Should().BeAssignableTo<IActionResult>();

            //compiler still sees result as a IActionResult. Explicit cast it.
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeAssignableTo<IEnumerable<JournalEntry>>();

            var model = viewResult.Model as IEnumerable<JournalEntry>;
            model.Should().HaveCount(2);
            model.Should().Contain(e => e.EntryName == "Test Entry" && e.EntryDetails == "Details");
            model.Should().Contain(e => e.EntryName == "Test Entry 2" && e.EntryDetails == "Details 2");
        }

        [Fact]
        public async Task JournalEntriesController_ShowSearchForm_ReturnsShowSearchFormView()
        {
            //Arrange - what do I need to bring it in?
            Initialize(Guid.NewGuid().ToString());

            //Act
            var result = await _controller.Index();

            //Assert
            // while the def for method says IActionResult,
            // it returns a ViewResult, a subtype. keeps it flexible.
            result.Should().BeOfType<ViewResult>();
            result.Should().BeAssignableTo<IActionResult>();

            //compiler still sees result as a IActionResult. Explicit cast it.
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().BeNull(); 
            // Expected when you return View("ShowSearchForm") from ShowSearchForm method (same name)
        }

        [Fact]
        public async Task JournalEntriesController_ShowSearchResults_ReturnsViewResultWithSearchResults()
        {
            //Arrange - what do I need to bring it in?
            // Use a unique DB name per test
            Initialize(Guid.NewGuid().ToString());
            string searchPhrase = "Test Entry 2";

            //Act
            var result = await _controller.ShowSearchResults(searchPhrase);

            //Assert
            // while the def for index says IActionResult,
            // it returns a ViewResult, a subtype. keeps it flexible.
            result.Should().BeOfType<ViewResult>();
            result.Should().BeAssignableTo<IActionResult>();

            //compiler still sees result as a IActionResult. Explicit cast it.
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeAssignableTo<IEnumerable<JournalEntry>>();

            var model = viewResult.Model as IEnumerable<JournalEntry>;
            model.Should().HaveCount(1);
            model.Should().Contain(e => e.EntryName == "Test Entry 2" && e.EntryDetails == "Details 2");
        }

    }
}

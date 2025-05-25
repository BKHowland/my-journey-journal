using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using my_journey_journal.Data;

namespace my_journey_journal.Tests.ControllerTests
{
    public class JournalEntriesControllerTests
    {
        private ApplicationDbContext _context;
        public JournalEntriesControllerTests()
        {
            // Creating mock for all dependencies in the controller.
            _context = A.Fake<ApplicationDbContext>();
        }
    }
}

using NetChallenge.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NetChallenge.Test
{
    public class OthersTests : OfficeRentalServiceTest
    {
        public OthersTests()
        {
            Service.AddLocation(AddLocationRequestMother.Default);
            Service.AddLocation(AddLocationRequestMother.Central);
            Service.AddOffice(AddOfficeRequestMother.Default);
            Service.AddOffice(AddOfficeRequestMother.Blue);
            Service.AddOffice(AddOfficeRequestMother.Red);
        }

        [Fact]
        public void NoThrowWhenEndingAndStartingAtSameTime()
        {
            var date = BookOfficeRequestMother.Default.DateTime;
            var request1 = BookOfficeRequestMother.Default;
            var request2 = BookOfficeRequestMother.Default.WithDate(date.AddHours(-1)).WithDuration(TimeSpan.FromHours(1));

            Service.BookOffice(request1);
            Service.BookOffice(request2);

            Assert.Equal(2, BookingRepository.AsEnumerable().Count());
        }

        [Fact]
        public void ThrowExceptionForInvalidDuration()
        {
            var request = BookOfficeRequestMother.Default.WithDuration(TimeSpan.FromHours(2.5));

            Assert.ThrowsAny<Exception>(() => Service.BookOffice(request));
        }

    }
}

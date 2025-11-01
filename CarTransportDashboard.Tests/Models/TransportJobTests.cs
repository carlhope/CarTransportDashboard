using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Models
{
    public class TransportJobTests
    {
        private readonly Vehicle _vehicle = new Vehicle(); // Stub or mock as needed
        private readonly ApplicationUser _driver = new ApplicationUser { Id = "driver-123", FirstName="test", LastName="user" };

        private TransportJob CreateAvailableJob()
        {
            return new TransportJob(
                title: "Test Job",
                description: "Deliver vehicle",
                pickupLocation: "A",
                dropoffLocation: "B",
                scheduledDate: DateTime.UtcNow.AddDays(1),
                assignedVehicleId: Guid.NewGuid(),
                vehicle: _vehicle
            );
        }
        [Fact]
        public void AssignDriver_SetsDriverAndStatus_WhenJobIsAvailable()
        {
            var job = CreateAvailableJob();

            job.AssignDriver(_driver);

            Assert.Equal(_driver, job.AssignedDriver);
            Assert.Equal(_driver.Id, job.AssignedDriverId);
            Assert.Equal(JobStatus.Allocated, job.Status);
            Assert.NotNull(job.AssignedAt);
            Assert.NotNull(job.UpdatedAt);
        }
        [Fact]
        public void UnassignDriver_ClearsDriverAndSetsStatusToAvailable_WhenJobIsAllocated()
        {
            var job = CreateAvailableJob();
            job.AssignDriver(_driver);

            job.UnassignDriver();

            Assert.Null(job.AssignedDriver);
            Assert.Null(job.AssignedDriverId);
            Assert.Equal(JobStatus.Available, job.Status);
            Assert.NotNull(job.UpdatedAt);
        }
        [Fact]
        public void AcceptJob_SetsStatusToInProgress_WhenJobIsAllocated()
        {
            var job = CreateAvailableJob();
            job.AssignDriver(_driver);

            job.AcceptJob();

            Assert.Equal(JobStatus.InProgress, job.Status);
            Assert.NotNull(job.AcceptedAt);
            Assert.NotNull(job.UpdatedAt);
        }
        [Fact]
        public void MarkAsCompleted_SetsStatusToCompleted_WhenJobIsInProgress()
        {
            var job = CreateAvailableJob();
            job.AssignDriver(_driver);
            job.AcceptJob();

            job.MarkAsCompleted();

            Assert.Equal(JobStatus.Completed, job.Status);
            Assert.NotNull(job.CompletedAt);
            Assert.NotNull(job.UpdatedAt);
        }
        [Fact]
        public void Cancel_SetsStatusToCancelled_WhenJobIsNotCompletedOrAlreadyCancelled()
        {
            var job = CreateAvailableJob();
            job.AssignDriver(_driver);

            job.Cancel();

            Assert.Equal(JobStatus.Cancelled, job.Status);
            Assert.Null(job.AssignedDriver);
            Assert.Null(job.AssignedDriverId);
            Assert.NotNull(job.UpdatedAt);
        }
    }

}

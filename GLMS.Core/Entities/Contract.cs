using GLMS.Core.Enums;
using GLMS.Core.Observers;
using System;
using System.Collections.Generic;

namespace GLMS.Core.Entities
{
    public class Contract : ISubject
    {
        private List<IObserver> _observers = new();

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public string ServiceLevel { get; set; } = string.Empty;

        public string? SignedAgreementPath { get; set; }

        public int ClientId { get; set; }

        public Client? Client { get; set; }

        public List<ServiceRequest> ServiceRequests { get; set; } = new();

        // ---------------- OBSERVER IMPLEMENTATION ----------------

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update($"Contract {Id} status changed to {Status}");
            }
        }

        // Optional helper
        public void ChangeStatus(ContractStatus newStatus)
        {
            Status = newStatus;
            Notify();
        }
    }
}
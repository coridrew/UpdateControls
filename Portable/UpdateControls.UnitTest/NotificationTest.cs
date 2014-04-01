using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KnockoutCS.UnitTest
{
    public class NotifyingObservable : Observable
    {
        public event Action OnGainDependent;
        public event Action OnLoseDependent;

        protected override void GainDependent()
        {
            if (OnGainDependent != null)
                OnGainDependent();
        }

        protected override void LoseDependent()
        {
            if (OnLoseDependent != null)
                OnLoseDependent();
        }
    }

    [TestClass]
    public class NotificationTest
    {
        private bool _gained;
        private bool _lost;
        private NotifyingObservable _observable;
        private Dependent _dependent;
        private Dependent _secondDependent;

        [TestInitialize]
        public void Initialize()
        {
            _gained = false;
            _observable = new NotifyingObservable();
            _observable.OnGainDependent += () => { _gained = true; };
            _observable.OnLoseDependent += () => { _lost = true; };
            _dependent = new Dependent(() => { _observable.OnGet(); });
            _secondDependent = new Dependent(() => { _observable.OnGet(); });
        }

        [TestMethod]
        public void DoesNotGainDependentOnCreation()
        {
            Assert.IsFalse(_gained, "The observable should not have gained a dependent.");
        }

        [TestMethod]
        public void GainsDependentOnFirstUse()
        {
            _dependent.OnGet();
            Assert.IsTrue(_gained, "The observable should have gained a dependent.");
        }

        [TestMethod]
        public void DoesNotGainDependentOnSecondUse()
        {
            _dependent.OnGet();
            _gained = false;
            _secondDependent.OnGet();
            Assert.IsFalse(_gained, "The observable should not have gained a dependent.");
        }

        [TestMethod]
        public void DoesNotLoseDependentOnCreation()
        {
            Assert.IsFalse(_lost, "The observable should not have lost a dependent.");
        }

        [TestMethod]
        public void DoesNotLoseDependentOnFirstUse()
        {
            _dependent.OnGet();
            Assert.IsFalse(_lost, "The observable should not have lost a dependent.");
        }

        [TestMethod]
        public void LosesDependentWhenChanging()
        {
            _dependent.OnGet();
            _observable.OnSet();
            Assert.IsTrue(_lost, "The observable should have lost a dependent.");
        }
    }
}

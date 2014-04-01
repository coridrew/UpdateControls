using System.Collections.Generic;
using KnockoutCS.Collections;

namespace KnockoutCS.UnitTest.ContactListData
{
    public class ContactList
    {
        private IndependentList<Contact> _contacts = new IndependentList<Contact>();

        public void AddContact(Contact contact)
        {
            _contacts.Add(contact);
        }

        public void DeleteContact(Contact contact)
        {
            _contacts.Remove(contact);
        }

        public IEnumerable<Contact> Contacts
        {
            get { return _contacts; }
        }
    }
}

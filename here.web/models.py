from google.appengine.ext import endpoints
from google.appengine.ext import ndb

class Notification(ndb.Model):
    timestamp = ndb.DateTimeProperty(auto_now=True)
    meeting = ndb.IntegerProperty()
    person = ndb.IntegerProperty()

class User(ndb.Model):
    token = ndb.StringProperty()
    name = ndb.StringProperty()

class ItemBase(ndb.Model):
    localId = ndb.IntegerProperty()
    status = ndb.IntegerProperty()
    parentId = ndb.IntegerProperty()

class Meeting(ItemBase):
    """Model to store meetings as uploaded by users by 
    """
    title = ndb.StringProperty(required=True)
    startTime = ndb.DateTimeProperty(required=True)
    internalId = ndb.IntegerProperty()
    originator = ndb.StringProperty()
    # player = ndb.UserProperty(required=True)
    @classmethod
    def get_meetings(cls, email, token):
        # return cls.query(ancestor=parent_key).order( -cls.date_created)
        persons = Person.get_persons(email, token);
        meetings = []
        for person in persons:
            meeting = cls.get_by_id(person.parentId)
            meetings.append(meeting)
        return meetings

class Person(ItemBase):
    """Model to store people related to a meeting as uploaded by users by 
    """
    name = ndb.StringProperty(required=True)
    email = ndb.StringProperty(required=True)

    @classmethod
    def get_persons(cls, email, token):
        # return cls.query(ancestor=parent_key).order( -cls.date_created)
        persons = cls.query(cls.email == email).fetch()
        return persons

class Role(ItemBase):
    """Model to store roles
    """
    name = ndb.StringProperty(required=True)
    importance = ndb.IntegerProperty(required=True)
import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from models import Meeting, Person, Role, User
import datetime

def meeting2Msg(meeting):
    res = MeetingMsg()
    res.id = meeting.key.id()
    res.title = meeting.title
    res.startTime = meeting.startTime
    res.internalId = meeting.internalId # the local id at the originator's device
    res.originator = meeting.originator
    return res

def toMeeting(msg):
    mm = Meeting(title=msg.title, startTime=msg.startTime, internalId=msg.internalId, originator=msg.originator);
    return mm

def authorise(id, token):
    user = User.get_by_id(id)
    if not user:
        message = 'No user with the id "%s" exists.' % id
        raise endpoints.NotFoundException(message)
    if token != user.token:
        message = 'Invalid credentials for "%s" exists.' % id
        raise endpoints.UnauthorizedException(message)

class MeetingMsg(messages.Message):
    id = messages.IntegerField(1)
    title = messages.StringField(2)
    startTime = message_types.DateTimeField(3)
    internalId = messages.IntegerField(4) # the local id at the originator's device
    originator = messages.StringField(5)
    person = messages.StringField(6)
    token = messages.StringField(7)

MEETING_SECURE_RESOURCE = endpoints.ResourceContainer(
    MeetingMsg, 
    id = messages.IntegerField(2),
    person=messages.StringField(3),
    token=messages.StringField(4))

class MeetingsCollection(messages.Message):
    items = messages.MessageField(MeetingMsg, 1, repeated=True)

class PersonMsg(messages.Message):
    id = messages.IntegerField(1)
    name = messages.StringField(2)
    email = messages.StringField(3)
    parentId = messages.IntegerField(4) # the meeting

PERSON_SECURE_RESOURCE = endpoints.ResourceContainer(
    PersonMsg, 
    id = messages.IntegerField(2),
    person=messages.StringField(3),
    token=messages.StringField(4))

class PersonsCollection(messages.Message):
    items = messages.MessageField(PersonMsg, 1, repeated=True)

class RoleMsg(messages.Message):
    id = messages.IntegerField(1)
    name = messages.StringField(2)
    importance = messages.IntegerField(3)
    parentId = messages.IntegerField(4) # the meeting

ROLE_SECURE_RESOURCE = endpoints.ResourceContainer(
    RoleMsg, 
    id = messages.IntegerField(2),
    person=messages.StringField(3),
    token=messages.StringField(4))

class RolesCollection(messages.Message):
    items = messages.MessageField(RoleMsg, 1, repeated=True)

GET_RESOURCE = endpoints.ResourceContainer(
    # The request body should be empty.
    message_types.VoidMessage,
    # Accept one url parameter: and integer named 'id'
    id=messages.IntegerField(1, variant=messages.Variant.INT64))

SECURE_RESOURCE = endpoints.ResourceContainer(
    id=messages.IntegerField(1, variant=messages.Variant.INT64), 
    person=messages.StringField(2),
    token=messages.StringField(3))

GET_BY_STRING = endpoints.ResourceContainer(
    # The request body should be empty.
    message_types.VoidMessage,
    # Accept one url parameter: and integer named 'id'
    userId=messages.StringField(1))

@endpoints.api(name='meeting', version='v2')
class MeetingApi(remote.Service):
    @endpoints.method(
        # This method does not take a request message.
        SECURE_RESOURCE,
        # This method returns a GreetingCollection message.
        MeetingsCollection,
        path='meetings/{id}/{person}/{token}',
        http_method='GET',
        name='meetings.list')
    def list_meetings(self, request):
        authorise(request.person, request.token)
        meetings = Meeting.get_meetings(request.person, request.token)
        # meetings = Meeting.query().fetch()
        res = MeetingsCollection()
        for meeting in meetings:
            res.items.append(meeting2Msg(meeting))
        return res

    @endpoints.method(
        MEETING_SECURE_RESOURCE,
        MeetingMsg,
        path='meetings/{person}/{token}',
        http_method='POST',
        name='meetings.create')
    def create_meeting(self, request):
        authorise(request.person, request.token)
        mm = toMeeting(request) # Meeting(title=request.title, startTime=request.startTime);
        mm.put();
        res = meeting2Msg(mm)
        return res

    @endpoints.method(  MEETING_SECURE_RESOURCE,
                        MeetingMsg,
                        http_method='PUT',
                        path='meetings/{id}/{person}/{token}',
                        name='meetings.update')
    def update(self, request):
        authorise(request.person, request.token)
        mm = Meeting.get_by_id(request.id)
        if not mm:
            message = 'No meeting with the id "%s" exists.' % request.id
            raise endpoints.NotFoundException(message)

        mm.title = request.title
        mm.startTime = request.startTime
        mm.put()
        res = MeetingMsg(id=mm.put().id(), title=mm.title, startTime=mm.startTime)
        return res

    @endpoints.method(SECURE_RESOURCE,
            message_types.VoidMessage,
            path='meetings/{id}/{person}/{token}',
            http_method='DELETE',
            name='meetings.delete')
    def delete(self, request):
        authorise(request.person, request.token)
        try:
            meeting = Meeting.get_by_id(request.id)
            if meeting.originator == request.person:
                persons = Person.query(Person.parentId==request.id).fetch()
                for person in persons:
                    roles = Role.query(Role.parentId==person.key.id()).fetch()
                    for role in roles:
                        role.key.delete()
                    person.key.delete()
                meeting.key.delete()
            else: # just remove the person from the meeting
                persons = Person.query(Person.parentId==request.id).fetch()
                for person in persons:
                    if(person.email == request.person):
                        roles = Role.query(Role.parentId==person.key.id()).fetch()
                        for role in roles:
                            role.key.delete()
                        person.key.delete()

            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))

@endpoints.api(name='person', version='v2')
class PersonApi(remote.Service):
    @endpoints.method(
        SECURE_RESOURCE,
        # This method returns a GreetingCollection message.
        PersonsCollection,
        path='persons/{id}/{person}/{token}',
        http_method='GET',
        name='persons.list')
    def list_persons(self, request):
        authorise(request.person, request.token)
        persons = Person.query(Person.parentId==request.id).fetch()
        # meetings = Meeting.query().fetch()
        res = PersonsCollection()
        for person in persons:
            msg = PersonMsg(name=person.name, id=person.key.id(), email=person.email, parentId=request.id)
            res.items.append(msg)
        return res

    @endpoints.method(
        # This method accepts a request body containing a Greeting message
        # and a URL parameter specifying how many times to multiply the
        # message.
        PERSON_SECURE_RESOURCE,
        # This method returns a Greeting message.
        PersonMsg,
        path='persons/{person}/{token}',
        http_method='POST',
        name='persons.create')
    def create_person(self, request):
        authorise(request.person, request.token)
        mm = Person(name=request.name, email=request.email, parentId=request.parentId);
        res = PersonMsg(id=mm.put().id(), name=request.name, email=request.email, parentId=request.parentId)
        return res

    @endpoints.method(SECURE_RESOURCE,
                    message_types.VoidMessage,
                    path='persons/{id}/{person}/{token}',
                    http_method='DELETE',
                    name='persons.delete')
    def delete(self, request):
        authorise(request.person, request.token)
        try:
            person = Person.get_by_id(request.id)
            roles = Role.query(Role.parentId==request.id).fetch()
            for role in roles:
                role.key.delete()
            person.key.delete()
            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))

@endpoints.api(name='role', version='v2')
class RoleApi(remote.Service):
    @endpoints.method(
        # This method does not take a request message.
        GET_RESOURCE,
        # This method returns a GreetingCollection message.
        RolesCollection,
        path='roles/{id}',
        http_method='GET',
        name='roles.list')
    def list_roles(self, request):
        authorise(request.person, request.token)
        roles = Role.query(Role.parentId==request.id).fetch()
        # meetings = Meeting.query().fetch()
        res = RolesCollection()
        for role in roles:
            msg = RoleMsg(name=role.name, id=role.key.id(), importance=role.importance, parentId=role.parentId)
            res.items.append(msg)
        return res

    @endpoints.method(
        # This method accepts a request body containing a Greeting message
        # and a URL parameter specifying how many times to multiply the
        # message.
        ROLE_SECURE_RESOURCE,
        # This method returns a Greeting messagee
        RoleMsg,
        path='roles',
        http_method='POST',
        name='roles.create')
    def create_meeting(self, request):
        authorise(request.person, request.token)
        mm = Role(name=request.name, importance=request.importance, parentId=request.parentId);
        res = RoleMsg(id=mm.put().id(), name=request.name, importance=request.importance, parentId=role.parentId)
        return res

    @endpoints.method(SECURE_RESOURCE,
                      message_types.VoidMessage,
                      path='roles/{id}/{person}/{token}',
                      http_method='DELETE',
                      name='roles.delete')
    def delete(self, request):
        authorise(request.person, request.token)
        try:
            role = Role.get_by_id(request.id)
            role.key.delete();
            return message_types.VoidMessage()

        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))
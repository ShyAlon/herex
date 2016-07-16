import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from models import Meeting, Person, Role
import datetime

class MeetingMsg(messages.Message):
    id = messages.IntegerField(1)
    title = messages.StringField(2)
    startTime = message_types.DateTimeField(3)

class MeetingsCollection(messages.Message):
    items = messages.MessageField(MeetingMsg, 1, repeated=True)

class PersonMsg(messages.Message):
    id = messages.IntegerField(1)
    name = messages.StringField(2)
    email = messages.StringField(3)
    meeting = messages.IntegerField(4)

class PersonsCollection(messages.Message):
    items = messages.MessageField(PersonMsg, 1, repeated=True)

class RoleMsg(messages.Message):
    id = messages.IntegerField(1)
    name = messages.StringField(2)
    importance = messages.IntegerField(3)
    person = messages.IntegerField(4)

class RolesCollection(messages.Message):
    items = messages.MessageField(RoleMsg, 1, repeated=True)

GET_RESOURCE = endpoints.ResourceContainer(
    # The request body should be empty.
    message_types.VoidMessage,
    # Accept one url parameter: and integer named 'id'
    id=messages.IntegerField(1, variant=messages.Variant.INT32))

GET_BY_STRING = endpoints.ResourceContainer(
    # The request body should be empty.
    message_types.VoidMessage,
    # Accept one url parameter: and integer named 'id'
    userId=messages.StringField(1))

@endpoints.api(name='meeting', version='v1')
class MeetingApi(remote.Service):
    @endpoints.method(
        # This method does not take a request message.
        GET_BY_STRING,
        # This method returns a GreetingCollection message.
        MeetingsCollection,
        path='meetings/{userId}',
        http_method='GET',
        name='meetings.list')
    def list_meetings(self, request):
        meetings = Meeting.get_meetings(request.userId, 'token')
        # meetings = Meeting.query().fetch()
        res = MeetingsCollection()
        for meeting in meetings:
            msg = MeetingMsg(title='Shy', startTime=datetime.datetime.now(), id=meeting.key.id())
            res.items.append(msg)
        return res

    @endpoints.method(
        # This method accepts a request body containing a Greeting message
        # and a URL parameter specifying how many times to multiply the
        # message.
        MeetingMsg,
        # This method returns a Greeting message.
        MeetingMsg,
        path='meetings',
        http_method='POST',
        name='meetings.create')
    def create_meeting(self, request):
        mm = Meeting(title=request.title, startTime=request.startTime);
        res = MeetingMsg(id=mm.put().id(), title=request.title, startTime=request.startTime)
        return res

    @endpoints.method(GET_RESOURCE,
            message_types.VoidMessage,
            path='meetings/{id}',
            http_method='DELETE',
            name='meetings.delete')
    def delete(self, request):
        try:
            meeting = Meeting.get_by_id(request.id)
            persons = Person.query(Person.parentId==request.id).fetch()
            for person in persons:
                roles = Role.query(Role.parentId==person.key.id()).fetch()
                for role in roles:
                    role.key.delete()
                person.key.delete()
            meeting.key.delete()
            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))

@endpoints.api(name='person', version='v1')
class PersonApi(remote.Service):
    @endpoints.method(
        # This method does not take a request message.
        GET_RESOURCE,
        # This method returns a GreetingCollection message.
        PersonsCollection,
        path='persons/{id}',
        http_method='GET',
        name='persons.list')
    def list_persons(self, request):
        persons = Person.query(Person.parentId==request.id).fetch()
        # meetings = Meeting.query().fetch()
        res = PersonsCollection()
        for person in persons:
            msg = PersonMsg(name=person.name, id=person.key.id(), email=person.email, meeting=request.id)
            res.items.append(msg)
        return res

    @endpoints.method(
        # This method accepts a request body containing a Greeting message
        # and a URL parameter specifying how many times to multiply the
        # message.
        PersonMsg,
        # This method returns a Greeting message.
        PersonMsg,
        path='persons',
        http_method='POST',
        name='persons.create')
    def create_person(self, request):
        mm = Person(name=request.name, email=request.email, parentId=request.meeting);
        res = PersonMsg(id=mm.put().id(), name=request.name, email=request.email, meeting=request.meeting)
        return res

    @endpoints.method(GET_RESOURCE,
                    message_types.VoidMessage,
                    path='persons/{id}',
                    http_method='DELETE',
                    name='persons.delete')
    def delete(self, request):
        try:
            person = Person.get_by_id(request.id)
            roles = Role.query(Role.parentId==request.id).fetch()
            for role in roles:
                role.key.delete()
            person.key.delete()
            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))

@endpoints.api(name='role', version='v1')
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
        roles = Role.query(Role.parentId==request.id).fetch()
        # meetings = Meeting.query().fetch()
        res = RolesCollection()
        for role in roles:
            msg = RoleMsg(name=person.name, id=person.key.id(), importance=role.importance, person=request.id)
            res.items.append(msg)
        return res

    @endpoints.method(
        # This method accepts a request body containing a Greeting message
        # and a URL parameter specifying how many times to multiply the
        # message.
        RoleMsg,
        # This method returns a Greeting messagee
        RoleMsg,
        path='roles',
        http_method='POST',
        name='roles.create')
    def create_meeting(self, request):
        mm = Role(name=request.name, importance=request.importance, parentId=request.person);
        res = RoleMsg(id=mm.put().id(), name=request.name, importance=request.importance, person=request.person)
        return res

    @endpoints.method(GET_RESOURCE,
                      message_types.VoidMessage,
                      path='roles/{id}',
                      http_method='DELETE',
                      name='roles.delete')
    def delete(self, request):
        try:
            role = Role.get_by_id(request.id)
            role.key.delete();
            return message_types.VoidMessage()

        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))
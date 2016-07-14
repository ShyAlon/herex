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


STORED_MEETINGS = MeetingsCollection(items=[
    MeetingMsg(title='going to eilat'),
    MeetingMsg(title='getting back'),
])


@endpoints.api(name='meeting', version='v1')
class MeetingApi(remote.Service):

    GET_RESOURCE = endpoints.ResourceContainer(
        # The request body should be empty.
        message_types.VoidMessage,
        # Accept one url parameter: and integer named 'id'
        userId=messages.IntegerField(1, variant=messages.Variant.INT32))

    @endpoints.method(
        # This method does not take a request message.
        message_types.VoidMessage,
        # This method returns a GreetingCollection message.
        MeetingsCollection,
        path='meetings',
        http_method='GET',
        name='meetings.list')
    def list_meetings(self, unused_request):
        meetings = Meeting.get_meetings('shy.alon@gmail.com', 'token')
        # meetings = Meeting.query().fetch()
        res = MeetingsCollection()
        for meeting in meetings:
            msg = MeetingMsg(title='Shy', startTime=datetime.datetime.now())
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

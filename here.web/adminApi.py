import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from models import Meeting, Person, Role, User
import datetime

class UserMsg(messages.Message):
    name = messages.StringField(1)

USER_SECURE_RESOURCE = endpoints.ResourceContainer(
    UserMsg,
    id = messages.StringField(2),
    token=messages.StringField(3))

@endpoints.api(name='user', version='v2')
class AdminApi(remote.Service):
    @endpoints.method(  USER_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='users/{id}/{token}',
                        http_method='POST',
                        name='users.create')
    def create(self, request):
        user = User(id=request.id, token=request.token, name=request.name)
        user.put()
        return message_types.VoidMessage()

    @endpoints.method(  USER_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        http_method='PUT',
                        path='users/{id}/{token}',
                        name='users.update')
    def update(self, request):
        user = User.get_by_id(request.id)
        if not user:
            message = 'No user with the id "%s" exists.' % request.id
            raise endpoints.NotFoundException(message)

        user.name = request.name
        user.token = request.token
        user.put()
        return message_types.VoidMessage()

    @endpoints.method(  USER_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='users/{id}/{token}',
                        http_method='DELETE',
                        name='users.delete')
    def delete(self, request):
        try:
            user = User.get_by_id(request.id)
            if user.token == request.token:
                user.key.delete()
            else: # just remove the person from the meeting
                raise endpoints.NotFoundException('Invalid token.')
            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('Role %s not found.' % (request.id,))
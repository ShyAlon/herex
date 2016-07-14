from protorpc import messages
from protorpc import message_types

class CheckListItemRepr(messages.Message):
    title = messages.StringField(1)
    checked = messages.BooleanField(2)

class ReprBase(messages.Message):
    key = messages.StringField(1)

class MeetingRepr(ReprBase):
    title = messages.StringField(2)
    content = messages.StringField(3)
    startTime = message_types.DateTimeField(4)

class MeetingCollection(messages.Message):
    items = messages.MessageField(MeetingRepr, 1, repeated=True)

class PersonRepr(ReprBase):
    name = messages.StringField(2)
    email = messages.StringField(3)

class RoleRepr(ReprBase):
    name = messages.StringField(2)
    email = messages.StringField(3)

class Role(ItemBase):
    """Model to store roles
    """
    name = ndb.StringProperty(required=True)
    importance = ndb.StringProperty(required=True) 
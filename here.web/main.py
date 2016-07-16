import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from meetingsApi import MeetingApi, PersonApi, RoleApi

import webapp2

EVENTS_PREFIX = "/events/"


class MainHandler(webapp2.RequestHandler):
    def get(self):
        self.response.write("<h1>Home!</h1>")


class CronEventHandler(webapp2.RequestHandler):
    def get(self):
        print(self.request.path)
        # topic_name = self.request.path.split(EVENTS_PREFIX)[-1]
        # publish_to_topic(topic_name, msg='test')
        self.response.status = 204

    def post(self):
        print(self.request.path)
        # topic_name = self.request.path.split(EVENTS_PREFIX)[-1]
        # publish_to_topic(topic_name, msg='test')
        self.response.status = 200


app = webapp2.WSGIApplication([('/', MainHandler),
                               ('/events/.*', CronEventHandler), ],
                              debug=True)

api = endpoints.api_server([MeetingApi, PersonApi, RoleApi])
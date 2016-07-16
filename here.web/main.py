import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from meetingsApi import MeetingApi, PersonApi, RoleApi
from models import Meeting, Person, Role
from google.appengine.ext import ndb
import logging
import webapp2
import datetime

EVENTS_PREFIX = "/events/"


class MainHandler(webapp2.RequestHandler):
    def get(self):
        self.response.write("<h1>Home!</h1>")


class CronEventHandler(webapp2.RequestHandler):
    def get(self):
        print(self.request.path)
        self.trackEvents()
        self.response.status = 200

    def post(self):
        print(self.request.path)
        self.response.status = 200

    """
    Iterate through the persisted events and act accordingly
    """
    def trackEvents(self):
        logging.info('Tracking Events')
        date = datetime.datetime.now()
        logging.info(date)
        immediate = Meeting.query(ndb.AND(Meeting.startTime < date),
                                Meeting.startTime >= date - datetime.timedelta(hours=1)).fetch()
        logging.info('Found {} meetings'.format(len(immediate)))
        for meeting in immediate:
            self.processMeeting(meeting)

    def processMeeting(self, meeting):
        logging.info('Processing {}'.format(meeting.title))
        participants = Person.query(Person.parentId == meeting.key.id()).fetch()
        for person in participants:
            self.processParticipant(person, meeting)
        
    def processParticipant(self, person, meeting):
        logging.info('Processing {}'.format(person.name))


app = webapp2.WSGIApplication([('/', MainHandler),
                               ('/events/.*', CronEventHandler), ],
                              debug=True)

api = endpoints.api_server([MeetingApi, PersonApi, RoleApi])
from meetingsApi import MeetingApi, PersonApi, RoleApi
from adminApi import AdminApi
from models import Meeting, Person, Role, Notification
from google.appengine.ext import ndb
import endpoints
import logging
import webapp2
import datetime
from gcm import GCM

EVENTS_PREFIX = "/events/"

API_KEY = "AIzaSyD8Hu1LCDS9ULG5UwgCdbt7pQACIR0BGMk"

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
        immediate = Meeting.query(ndb.AND(Meeting.startTime >= date + datetime.timedelta(minutes=50)),
                                Meeting.startTime < date + datetime.timedelta(hours=1)).fetch()
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
        note = Notification(meeting=meeting.key.id(), person=person.key.id())
        note.put();

        gcm = GCM(API_KEY, debug=True)
        data = {'meeting': meeting.key.id(), 'person': person.key.id()}
        topic = 'meeting' + str(meeting.key.id())
        response = gcm.send_topic_message(topic=topic, data=data)
        logging.info(response)

app = webapp2.WSGIApplication([('/', MainHandler),
                               ('/events/.*', CronEventHandler), ],
                              debug=True)

api = endpoints.api_server([MeetingApi, PersonApi, RoleApi, AdminApi])
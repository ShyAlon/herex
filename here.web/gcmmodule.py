#!/usr/bin/env python
# appengine-apns-gcm was developed by Garett Rogers <garett.rogers@gmail.com>
# Source available at https://github.com/GarettRogers/appengine-apns-gcm
#
# appengine-apns-gcm is distributed under the terms of the MIT license.
#
# Copyright (c) 2013 AimX Labs
#
# Permission is hereby granted, free of charge, to any person obtaining a copy of
# this software and associated documentation files (the "Software"), to deal in
# the Software without restriction, including without limitation the rights to
# use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
# of the Software, and to permit persons to whom the Software is furnished to do
# so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

import webapp2
import os
from google.appengine.ext.webapp import template
from google.appengine.ext import ndb
from gcmdata import *
from gcm import *
from apns import *
from apnsdata import *
from appdata import *
import endpoints
from protorpc import message_types
from protorpc import messages
from protorpc import remote
from models import User

appconfig = None

def authorise(id, token):
    user = User.get_by_id(id)
    if not user:
        message = 'No user with the id "%s" exists.' % id
        raise endpoints.NotFoundException(message)
    if token != user.token:
        message = 'Invalid credentials for "%s" exists.' % id
        raise endpoints.UnauthorizedException(message)


GCM_SECURE_RESOURCE = endpoints.ResourceContainer(
    person = messages.StringField(1),
    token = messages.StringField(2),
    regid = messages.StringField(3),
    tagid = messages.StringField(4))

@endpoints.api(name='gcm', version='v2')
class GCMApi(remote.Service):
    @endpoints.method(  GCM_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='gcmregister/{person}/{token}/{regid}/{tagid}',
                        http_method='POST',
                        name='gcm.register')
    def register(self, request):
        authorise(request.person, request.token)
        regid = request.regid
        if not regid:
            self.response.out.write('Must specify regid')
        else:
            token = GcmToken.get_or_insert(regid)
            token.gcm_token = regid
            token.enabled = True
            token.put()
        return message_types.VoidMessage()


    @endpoints.method(  GCM_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='gcmunregister/{person}/{token}/{regid}/{tagid}',
                        http_method='POST',
                        name='gcm.unregister')
    def unregister(self, request):
        authorise(request.person, request.token)
        regid = request.regid
        token = GcmToken.get_or_insert(regid)
        token.enabled = False
        token.put()
        return message_types.VoidMessage()

    @endpoints.method(  GCM_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='gcmtaghandler/{person}/{token}/{regid}/{tagid}',
                        http_method='POST',
                        name='gcm.taghandler')
    def taghandler(self, request):
        authorise(request.person, request.token)
        tagid = request.tagid
        regid = request.regid
        
        appconfig = AppConfig.get_or_insert("config")       
        token = GcmToken.get_or_insert(regid)
        token.gcm_token = regid
        token.put()
        logging.info(token)
        tag = GcmTag.get_or_insert(tagid + regid, tag=tagid, token=token.key)
        logging.info(tag)
        return message_types.VoidMessage()

    @endpoints.method(  GCM_SECURE_RESOURCE,
                        message_types.VoidMessage,
                        path='gcmdeletetag/{person}/{token}/{regid}/{tagid}',
                        http_method='DELETE',
                        name='gcm.deletetag')
    def delete(self, request):
        authorise(request.person, request.token)
        try:
            tagid = request.tagid
            regid = request.regid
            appconfig = AppConfig.get_or_insert("config")
            tag = GcmTag.get_or_insert(tagid + regid)
            tag.key.delete()
            return message_types.VoidMessage()
        except (IndexError, TypeError):
            raise endpoints.NotFoundException('tag %s not found.' % (request.id,))

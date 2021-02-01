#!/bin/python

# Copyright 2021 MASES s.r.l.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#
# Refer to LICENSE for more information.

import sys,getopt,ftplib

optDict = dict()

def usage():
    print ('upload.py -c <check|upload|download> -s <server> -u <user> -p <password> -l <localfile> -r <remotefile>')


def main(argv):
    try:
        opts, args = getopt.getopt(argv,'hc:s:u:p:l:r:')
    except getopt.GetoptError:
        usage()
        sys.exit(2)
    for opt, arg in opts:
        if opt == '-h':
            usage()
            sys.exit()
        optDict[opt] = arg

    session = ftplib.FTP(optDict['-s'],optDict['-u'],optDict['-p'])
    if(optDict['-c'] == 'check'):
        for name in session.nlst():                             # check the file
            if (optDict['-r'] == name):
                print ('File exists')
                session.quit()                                  # close FTP
                sys.exit(0)                              
        session.quit()                                          # close FTP
        sys.exit(1)
    elif (optDict['-c'] == 'upload'):
        file = open(optDict['-l'],'rb')                         # file to send
        session.storbinary('STOR ' + optDict['-r'], file)       # send the file
        file.close()                                            # close file and FTP
        session.quit()
    elif(optDict['-c'] == 'download'):
        session.retrbinary('RETR ' + optDict['-r'], open(optDict['-l'], 'wb').write)    # retrieve the file
        session.quit()                                          # close FTP
    

if __name__ == "__main__":
   main(sys.argv[1:])

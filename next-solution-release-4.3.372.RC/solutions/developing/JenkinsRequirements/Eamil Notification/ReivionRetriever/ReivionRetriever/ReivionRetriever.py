
from aifc import Error
from asyncio.windows_events import NULL
from audioop import reverse
from dataclasses import replace
from itertools import product
from logging import lastResort
from operator import contains, countOf
from pickle import APPEND
from pydoc import resolve
from re import sub
from smtpd import SMTPServer
import sys
from this import s
#import mariadb
import json
import urllib.request
from urllib.parse import urlencode
from fogbugz import FogBugz
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
import re
import os
# fogbugz tags

# fill in the following values as appropriate to fogbugz installation
fogbugzURL      = 'https://support.progea.com/Products/'
fogbugzUsername = sys.argv[1]
fogbugzPassword = sys.argv[2]
# fill in the following values as appropriate to kiln installation
kilnURL         = 'https://support.progea.com/Products/Kiln/Api/2.0/'

# constanbts to retrive rpositories id
MoviconSName    = 'Movicon.NExT'
DriversSName    = 'Drivers.NExT'
MoviconNextSlug = 'Movicon-NExT'
DriversNextSlug = 'Drivers-NExT'

#Json keys
MoviconNextStartRevNumber   = "MoviconNextStartRevNumber"
SourcesSartRevNumber        = "SourcesSartRevNumber"
DriversStartRevisionNumber  = "DriversStartRevisionNumber"
RevisionNumberInfo           = "RevisionNumberInfo"

# managing instring chars
def in_(s, other):
    return other in s

# managing kiln request and response
def slurp(url, params={}, post = False, raw=False):
    params = urlencode(params, doseq=True)
    handle = urllib.request.urlopen(url, params) if post else urllib.request.urlopen(url + '?' + params)
    content = handle.read()
    obj = content if raw else json.loads(content)
    handle.close()
    return obj

# managing kiln api call
def api(url):
    return kilnURL + url


# conversion of a dict in JSON
def toJSON(dictToConvert):
    return json.dumps(dictToConvert, indent=4, sort_keys=True, default=str)
def GetIXRpos(projects):
    ixRepos = {}
    for p in projects:
        projectSName = p['sName']
        if projectSName == MoviconSName or projectSName == DriversSName:
             for g in p['repoGroups']:
                 if g['sName'] == 'Progea':
                     for r in g['repos']:
                         sProjectSlug = r['sProjectSlug']
                         if sProjectSlug == MoviconNextSlug or sProjectSlug == DriversNextSlug:
                             if not contains(ixRepos.keys(),sProjectSlug):
                                 ixRepos[sProjectSlug] = []
                             ixRepoAliases = r['rgAliases']
                             if contains(ixRepoAliases,'Developing') or contains(ixRepoAliases,'Developing-Sources'):
                                ixRepos[sProjectSlug].append(r['ixRepo'])
                                if sProjectSlug == DriversNextSlug:
                                    ixRepos[MoviconNextSlug].reverse()
                                    return ixRepos

    # Start from movicon instead of sources
    return ixRepos

def GetEmailText(caselists):
    versioninfo = re.search('[0-9]+\\.[0-9]+', GetVersionInfo(sys.argv[4])).group(0)
    retvalue = """\
    <html>
        <head>
        </head>
        <body> 
            <p>
            A new update of Movicon.Next has been published.
            <br/><br/> 
                <a href="file://itmdnaspengfs01/UPDATE/Latest/mov{0}">\\\\itmdnaspengfs01\\UPDATE\\Latest\\mov{0}\\*.*
                </a>
           </p>
           <p>Following cases has been fixed.
           </p>""".format(versioninfo)
    
    for k in caselists.keys():
        if k == DriversNextSlug and caselists[k] != []: 
                retvalue += '</p>'
        retvalue +=  '<h3 style="margin-bottom:0;">' + k.replace('-','.') + ':</h3><p style="margin:0;padding:0;">'
        if caselists[k] == []:
            retvalue += '(<span style="font-weight: 800; font-size:14pt;">No case found.</span>)</p>'
        for c in caselists[k]:
            c = c.replace('>','&gt;').replace('<','&lt;')
            retvalue += '%s<br/>' %c
            
   
    retvalue += '</p></body></html>'
    return retvalue

def SendEmail(emailfrom, emailto , subject, body):
    message = MIMEMultipart('alternative')
    message['Subject'] = subject
    message['From'] = emailfrom
    message['To']   = emailto
    content = MIMEText(body,'html')
    message.attach(content)
    smtserver = smtplib.SMTP('Inetmail.emrsn.org')
    smtserver.sendmail(emailfrom,emailto,message.as_string())
    smtserver.quit()

def GetVersionInfo(AssemblyInfoFilePath, revisionnumber = 0):
    retvalue = None
    if os.path.isfile(AssemblyInfoFilePath):
        with open(AssemblyInfoFilePath,'r') as AssemblyInfoCs:
            retvalue = re.search('([0-9]+\\.){3}[0-9]+', re.search('\\[[a-z A-Z]+:\\s*[a-z A-Z]+\\(\\"([0-9]+\\.){3}[0-9]+', AssemblyInfoCs.read()).group(0))
    return retvalue.group(len( retvalue.groups()) - 1)

def GetSubject(AssemblyInfoFilePath,prefix ='Movicon.NExT'):
    
    with open('RevisionNumbers/Revisions.json','r') as jsonfilel:
           revnumbers = json.loads( jsonfilel.read())

    return prefix + ' ' + GetVersionInfo(AssemblyInfoFilePath, revnumbers["RevisionNumberInfo"])

#main function
def main():
    try:

        fb = FogBugz(fogbugzURL)
        fb.logon(fogbugzUsername, fogbugzPassword)
    except:
        
        sys.exit(0)
    
    # connecting to kiln
    try:
        kilnToken = slurp(api('Auth/Login'), dict(sUser=fogbugzUsername, sPassword=fogbugzPassword))
        ixRepos = GetIXRpos(slurp(api('Project'), dict (token=kilnToken)))
        caselists = {}
        revisions = []
        lastrev = None
        with open('RevisionNumbers/Revisions.json','r') as jsonfilel:
           revnumbers = json.loads( jsonfilel.read())

        for k,v in ixRepos.items():
            i = 0
            #revOldest = 31640, nChangesetLimit = 100
            while i < len(v):
                
                if not contains(caselists.keys(),k):
                    lastrepoentry = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken, nChangesetLimit = 1))[0]['rev']
                    lastrev = revnumbers[MoviconNextStartRevNumber] if k == MoviconNextSlug else revnumbers[DriversStartRevisionNumber] 
                    revisions = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken,revOldest = lastrev, nChangesetLimit = 100))
                                            
                    if k == MoviconNextSlug and revisions != None and revisions != []:
                        revnumbers[RevisionNumberInfo] += len(revisions) if revisions[0]['rev'] != revnumbers[MoviconNextStartRevNumber] else 0

                    if revisions != None and revisions != []:
                        lastrev = revisions[0]['rev']
                        revisions.remove(revisions[len(revisions) -1])
                        caselists[k] = [x['sDescription'] for x in revisions if (( 'MERGE' in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ) or ( 'MERGE' not in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ))]
                        
                    while lastrev != lastrepoentry and revisions != [] and revisions != None:
                        revisions = []
                        revisions = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken,revOldest = lastrev, nChangesetLimit = 100)) + revisions
                        if revisions != [] and revisions != None:
                            revisions.remove(revisions[len(revisions) -1])
                            if k == MoviconNextSlug:
                                revnumbers[RevisionNumberInfo] += len(revisions) if revisions[0]['rev'] != revnumbers[MoviconNextStartRevNumber] else 0
                            lastrev = revisions[0]['rev']
                            caselists[k] = [x['sDescription'] for x in revisions if (( 'MERGE' in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ) or ( 'MERGE' not in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ))] + caselists[k]                
                    
                    if k == MoviconNextSlug:
                        revnumbers[MoviconNextStartRevNumber] = lastrev 
                    else:
                        revnumbers[DriversStartRevisionNumber] = lastrev
                else:
                    revisions = []
                    lastrepoentry = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken, nChangesetLimit = 1))[0]['rev']
                    lastrev = revnumbers[SourcesSartRevNumber]
                    revisions = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken,revOldest = lastrev, nChangesetLimit = 100))
                    
                    if revisions != None and revisions != []:
                        lastrev = revisions[0]['rev']
                        revisions.remove(revisions[len(revisions) -1])
                        caselists[k] += [x['sDescription'] for x in revisions if (( 'MERGE' in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ) or ( 'MERGE' not in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ))]
                    
                    while lastrev != lastrepoentry and revisions != [] and revisions != None:
                        lastindex = len(revisions) - 1
                        revisions = []
                        revisions = slurp(api("repo/%d/History" % ixRepos[k][i]), dict (token=kilnToken,revOldest = lastrev, nChangesetLimit = 100)) + revisions
                        if revisions != [] and revisions != None:
                            revisions.remove(revisions[len(revisions) -1])
                            lastrev = revisions[0]['rev']
                            [caselists[k].insert(lastindex, x['sDescription']) for x in revisions if (( 'MERGE' in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ) or ( 'MERGE' not in x['sDescription'].upper() and 'CASE' in x['sDescription'].upper() ))]
                    
                    revnumbers[SourcesSartRevNumber] = lastrev
                               
                i = i + 1   

        revnumbersjson = toJSON(revnumbers)

        with open('RevisionNumbers/Revisions.json','w') as jsonfile:
            jsonfile.write(revnumbersjson)
        emailtext = GetEmailText(caselists) 
        SendEmail('jenkins@emerson.com',sys.argv[3],GetSubject(sys.argv[4].replace('\\','/')),emailtext)
    except Error as e:
        print(e.__str__())
        sys.exit(-1)
        
# check if the script is run as main module
if __name__ == '__main__':
    main()


import sys,json
import subprocess
from django.http import JsonResponse
from utility.utilityClass import RUNNING_TASK_MEMORY
from random import choice
from string import ascii_uppercase
import datetime
import linecache
from trainModel import kerasUtilities
kerasUtilities = kerasUtilities.KerasUtilities()
logFolder='./logs/'
statusfileLocation = ''

class CodeUtilityClass:

    def compileCode(filePath):
        import pathlib
        fullPath=pathlib.Path(filePath)
        filePath_ = fullPath.parent.__str__()
        fileName = fullPath.name
        sys.path.append(filePath_)
        try:
            exec("import "+fileName.replace('.py',''))
            return JsonResponse({"message":"Code Compiled Successfully"},status=200)
        except Exception as e:
            return JsonResponse({"message": str(e)},status=500)

    
    def executeCode(filePath,params):

        def updateStatusOfExecution(filePath,updatedStatus,info_dict):
            with open(filePath,'r') as sFile:
                sFileText=sFile.read()
            data_details=json.loads(sFileText)
            data_details['status']=updatedStatus
            data_details['information']=[]
            for key, val in info_dict.items():
                data_details['information'].append({
                    'property':key,
                    'value':val
                })

            if updatedStatus=='Complete':
                data_details['completedOn']=str(datetime.datetime.now())
            with open(filePath,'w') as filetosave:
                json.dump(data_details, filetosave)
            return 'Success'


        def monitorThread(filePath,args,statusfileLocation):
            print (args)
            args = [str(a) for a in args]
            popen = subprocess.Popen([sys.executable,filePath]+args,stdout=subprocess.PIPE,stderr=subprocess.PIPE)
            output,error = popen.communicate()
            if output:
                output=output.decode('utf-8')
                info = {'Output':output}
                updateStatusOfExecution(statusfileLocation,'Complete',info_dict=info)
            if error:
                error=error.decode('utf-8')
                info = {'Error':error}
                updateStatusOfExecution(statusfileLocation,'Execution Failed',info_dict=info)

        idforData=''.join(choice(ascii_uppercase) for i in range(12))
        saveStatus=logFolder+idforData+'/'
        kerasUtilities.checkCreatePath(saveStatus)
        statusfileLocation=saveStatus+'status.txt'
        with open(statusfileLocation,'w') as filetosave:
            json.dump({}, filetosave)

        # print ('>>>>>>>>>>>>>>>>',params)

        import threading
        pp = threading.Thread(target=monitorThread,args=(filePath,params,statusfileLocation))
        pp.start()
        pID = pp.ident
        import pathlib
        fullPath=pathlib.Path(filePath)
        fileName = fullPath.name.replace('.py','')
        tempRunMemory={'idforData': idforData,
			'status': 'Execution Failed' if pID==-1 else 'In Progress',
			'createdOn': str(datetime.datetime.now()),
			'type': 'Code',
			'pid':pID,
            'newPMMLFileName':fileName.split('/')[-1],
            'information':[
                {'property':'Parameters','value':params}
            ]
            }
        tempRunMemory['taskName']=tempRunMemory['newPMMLFileName']
        RUNNING_TASK_MEMORY.append(tempRunMemory)
        with open(statusfileLocation,'w') as filetosave:
            json.dump(tempRunMemory, filetosave)
        return JsonResponse(tempRunMemory,status=200)


class CodeUtilityClassNew:

    def compileCode(filePath):
        import pathlib
        fullPath=pathlib.Path(filePath)
        filePath_ = fullPath.parent.__str__()
        fileName = fullPath.name
        sys.path.append(filePath_)
        try:
            exec("import "+fileName.replace('.py',''))
            return JsonResponse({"message":"Code Compiled Successfully"},status=200)
        except Exception as e:
            return JsonResponse({"message": str(e)},status=500)

    
    def executeCode(filePath,params):

        def updateStatusOfExecution(filePath,updatedStatus,info_dict):
            with open(filePath,'r') as sFile:
                sFileText=sFile.read()
            data_details=json.loads(sFileText)
            data_details['status']=updatedStatus
            data_details['information']=[]
            for key, val in info_dict.items():
                data_details['information'].append({
                    'property':key,
                    'value':val
                })

            if updatedStatus=='Complete':
                data_details['completedOn']=str(datetime.datetime.now())
            with open(filePath,'w') as filetosave:
                json.dump(data_details, filetosave)
            return 'Success'


        def monitorThread(filePath,args,statusfileLocation):
            print (args)
            args = [str(a) for a in args]
            popen = subprocess.Popen([sys.executable,filePath]+args,stdout=subprocess.PIPE,stderr=subprocess.PIPE)
            output,error = popen.communicate()
            if output:
                output=output.decode('utf-8')
                info = {'Output':output}
                updateStatusOfExecution(statusfileLocation,'Complete',info_dict=info)
            if error:
                error=error.decode('utf-8')
                info = {'Error':error}
                updateStatusOfExecution(statusfileLocation,'Execution Failed',info_dict=info)

        idforData=''.join(choice(ascii_uppercase) for i in range(12))
        saveStatus=logFolder+idforData+'/'
        kerasUtilities.checkCreatePath(saveStatus)
        statusfileLocation=saveStatus+'status.txt'
        with open(statusfileLocation,'w') as filetosave:
            json.dump({}, filetosave)

        # print ('>>>>>>>>>>>>>>>>',params)

        import threading
        pp = threading.Thread(target=monitorThread,args=(filePath,params,statusfileLocation))
        pp.start()
        pID = pp.ident
        import pathlib
        fullPath=pathlib.Path(filePath)
        fileName = fullPath.name.replace('.py','')
        tempRunMemory={'idforData': idforData,
			'status': 'Execution Failed' if pID==-1 else 'In Progress',
			'createdOn': str(datetime.datetime.now()),
			'type': 'Code',
			'pid':pID,
            'newPMMLFileName':fileName.split('/')[-1],
            'information':[
                {'property':'Parameters','value':params}
            ]
            }
        tempRunMemory['taskName']=tempRunMemory['newPMMLFileName']
        RUNNING_TASK_MEMORY.append(tempRunMemory)
        with open(statusfileLocation,'w') as filetosave:
            json.dump(tempRunMemory, filetosave)
        return JsonResponse(tempRunMemory,status=200)

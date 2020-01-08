def postSriptCode2(scoreFrom1st):
    url2 = "https://ai.eu-latest.cumulocity.com/measurement/measurements"
    import time
    import random
    import requests
    import json
    from requests.auth import HTTPBasicAuth
    import datetime
    import numpy as np
    
    if 'ndarray' in str(type(scoreFrom1st)):
        predictions=scoreFrom1st
    else:
        predictions=[scoreFrom1st]

    for i in predictions:
        print(type(i[0]))
        tt=datetime.datetime.now()
        payload2={'type': 'iPhone',
        'time': str(tt.date())+'T'+str(tt.hour)+':'+str(tt.minute)+':'+str(tt.second)+'+05:30',
        'source': {'id': "114"},
        'Predictions_NN': {'Predictions_NN':{'value': np.float64(i[0])}}
        }
        headers = {
        'Content-Type': "application/json",
        'Accept': "application/vnd.com.nsn.cumulocity.measurement+json",
        'cache-control': "no-cache",
        'Postman-Token': "2d5fa27d-c8c8-428c-b2f9-0efe9490b716"
        }
        print(payload2)
        response = requests.request("POST", url2, data=json.dumps(payload2), 
                                headers=headers,auth=HTTPBasicAuth('vran', 'Testing@123'))


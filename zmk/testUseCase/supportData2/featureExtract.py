def addFeatureDataStruc(data,scoreFrom1st):
    data['LenCarName']=data['car name'].apply(lambda x: len(x.split()))
    data['lrScore']=scoreFrom1st
    newFeat=[ 'cylinders', 'displacement', 'horsepower', 'weight','acceleration','lrScore','LenCarName']
    return data[newFeat]
    
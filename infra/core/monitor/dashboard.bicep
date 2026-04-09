param name string
param location string
param tags object
param applicationInsightsId string
param applicationInsightsName string

resource dashboard 'Microsoft.Portal/dashboards@2020-09-01-preview' = {
  name: name
  location: location
  tags: tags
  properties: {
    lenses: [
      {
        order: 0
        parts: [
          {
            position: {
              x: 0
              y: 0
              colSpan: 6
              rowSpan: 4
            }
            metadata: {
              inputs: [
                { name: 'options', isOptional: true, value: {
                    chart: {
                      metrics: [
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'requests/count'
                          aggregationType: 7
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Server requests' }
                        }
                      ]
                      title: 'Server requests'
                      visualization: { chartType: 2 }
                    }
                  }
                }
                { name: 'sharedTimeRange', isOptional: true }
              ]
              #disable-next-line BCP036
              type: 'Extension/HubsExtension/PartType/MonitorChartPart'
            }
          }
          {
            position: {
              x: 6
              y: 0
              colSpan: 6
              rowSpan: 4
            }
            metadata: {
              inputs: [
                { name: 'options', isOptional: true, value: {
                    chart: {
                      metrics: [
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'requests/duration'
                          aggregationType: 4
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Server response time' }
                        }
                      ]
                      title: 'Server response time'
                      visualization: { chartType: 2 }
                    }
                  }
                }
                { name: 'sharedTimeRange', isOptional: true }
              ]
              #disable-next-line BCP036
              type: 'Extension/HubsExtension/PartType/MonitorChartPart'
            }
          }
          {
            position: {
              x: 0
              y: 4
              colSpan: 6
              rowSpan: 4
            }
            metadata: {
              inputs: [
                { name: 'options', isOptional: true, value: {
                    chart: {
                      metrics: [
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'requests/failed'
                          aggregationType: 7
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Failed requests' }
                        }
                      ]
                      title: 'Failed requests'
                      visualization: { chartType: 2 }
                    }
                  }
                }
                { name: 'sharedTimeRange', isOptional: true }
              ]
              #disable-next-line BCP036
              type: 'Extension/HubsExtension/PartType/MonitorChartPart'
            }
          }
          {
            position: {
              x: 6
              y: 4
              colSpan: 6
              rowSpan: 4
            }
            metadata: {
              inputs: [
                { name: 'options', isOptional: true, value: {
                    chart: {
                      metrics: [
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'browserTimings/totalDuration'
                          aggregationType: 4
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Browser page load time' }
                        }
                      ]
                      title: 'Browser page load time'
                      visualization: { chartType: 2 }
                    }
                  }
                }
                { name: 'sharedTimeRange', isOptional: true }
              ]
              #disable-next-line BCP036
              type: 'Extension/HubsExtension/PartType/MonitorChartPart'
            }
          }
          {
            position: {
              x: 0
              y: 8
              colSpan: 4
              rowSpan: 3
            }
            metadata: {
              inputs: [
                { name: 'ComponentId', value: { Name: applicationInsightsName, ResourceId: applicationInsightsId } }
                { name: 'ResourceIds', isOptional: true, value: [ applicationInsightsId ] }
              ]
              #disable-next-line BCP036
              type: 'Extension/AppInsightsExtension/PartType/AppMapGalPt'
            }
          }
          {
            position: {
              x: 4
              y: 8
              colSpan: 8
              rowSpan: 3
            }
            metadata: {
              inputs: [
                { name: 'options', isOptional: true, value: {
                    chart: {
                      metrics: [
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'exceptions/count'
                          aggregationType: 7
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Exceptions' }
                        }
                        {
                          resourceMetadata: { id: applicationInsightsId }
                          name: 'dependencies/failed'
                          aggregationType: 7
                          namespace: 'microsoft.insights/components'
                          metricVisualization: { displayName: 'Dependency failures' }
                        }
                      ]
                      title: 'Exceptions and dependency failures'
                      visualization: { chartType: 2 }
                    }
                  }
                }
                { name: 'sharedTimeRange', isOptional: true }
              ]
              #disable-next-line BCP036
              type: 'Extension/HubsExtension/PartType/MonitorChartPart'
            }
          }
        ]
      }
    ]
  }
}

output id string = dashboard.id
output name string = dashboard.name

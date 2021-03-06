﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Management.Sql;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Models;
using Microsoft.Azure.Commands.Sql.Common;
using Microsoft.Azure.Management.Sql.Models;

namespace Microsoft.Azure.Commands.Sql.ServerDisasterRecoveryConfiguration.Services
{
    /// <summary>
    /// This class is responsible for all the REST communication with the audit REST endpoints
    /// </summary>
    public class AzureSqlServerDisasterRecoveryConfigurationCommunicator
    {
        /// <summary>
        /// The Sql client to be used by this end points communicator
        /// </summary>
        private static SqlManagementClient SqlClient { get; set; }
        
        /// <summary>
        /// Gets or set the Azure subscription
        /// </summary>
        private static AzureSubscription Subscription {get ; set; }

        /// <summary>
        /// Gets or sets the Azure profile
        /// </summary>
        public AzureContext Context { get; set; }

        /// <summary>
        /// Creates a communicator for Azure Sql Server Disaster Recovery Configuration
        /// </summary>
        /// <param name="context"></param>
        public AzureSqlServerDisasterRecoveryConfigurationCommunicator(AzureContext context)
        {
            Context = context;
            if (context.Subscription != Subscription)
            {
                Subscription = context.Subscription;
                SqlClient = null;
            }
        }

        /// <summary>
        /// Gets the Azure Sql Server Disaster Recovery Configuration
        /// </summary>
        public Management.Sql.Models.ServerDisasterRecoveryConfiguration Get(string resourceGroupName, string serverName, string serverDisasterRecoveryConfigurationName, string clientRequestId)
        {
            return GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.Get(resourceGroupName, serverName, serverDisasterRecoveryConfigurationName).ServerDisasterRecoveryConfiguration;
        }

        /// <summary>
        /// Lists Azure Sql Server Disaster Recovery Configurations
        /// </summary>
        public IList<Management.Sql.Models.ServerDisasterRecoveryConfiguration> List(string resourceGroupName, string serverName, string clientRequestId)
        {
            return GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.List(resourceGroupName, serverName).ServerDisasterRecoveryConfigurations;
        }

        /// <summary>
        /// Creates a Disaster Recovery Configuration
        /// </summary>
        public Management.Sql.Models.ServerDisasterRecoveryConfiguration Create(string resourceGroupName, string serverName, string serverDisasterRecoveryConfigurationName, string clientRequestId, ServerDisasterRecoveryConfigurationCreateOrUpdateParameters parameters)
        {
            return GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.CreateOrUpdate(resourceGroupName, serverName, serverDisasterRecoveryConfigurationName, parameters).ServerDisasterRecoveryConfiguration;
        }

        /// <summary>
        /// Failover a Disaster Recovery Configuration
        /// </summary>
        public void Failover(string resourceGroupName, string serverName, string serverDisasterRecoveryConfigurationName, bool allowDataLoss, string clientRequestId)
        {
            if (allowDataLoss)
            {
                GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.FailoverAllowDataLoss(resourceGroupName, serverName, serverDisasterRecoveryConfigurationName);
            }
            else
            {
                GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.Failover(resourceGroupName, serverName, serverDisasterRecoveryConfigurationName);
            }
        }

        /// <summary>
        /// Deletes a Server Disaster Recovery Configuration
        /// </summary>
        public void Remove(string resourceGroupName, string serverName, string serverDisasterRecoveryConfigurationName, string clientRequestId)
        {
            GetCurrentSqlClient(clientRequestId).ServerDisasterRecoveryConfigurations.Delete(resourceGroupName, serverName, serverDisasterRecoveryConfigurationName);
        }

        /// <summary>
        /// Retrieve the SQL Management client for the currently selected subscription, adding the session and request
        /// id tracing headers for the current cmdlet invocation.
        /// </summary>
        /// <returns>The SQL Management client for the currently selected subscription.</returns>
        private SqlManagementClient GetCurrentSqlClient(String clientRequestId)
        {
            // Get the SQL management client for the current subscription
            if (SqlClient == null)
            {
                SqlClient = AzureSession.ClientFactory.CreateClient<SqlManagementClient>(Context, AzureEnvironment.Endpoint.ResourceManager);
            }
            SqlClient.HttpClient.DefaultRequestHeaders.Remove(Constants.ClientRequestIdHeaderName);
            SqlClient.HttpClient.DefaultRequestHeaders.Add(Constants.ClientRequestIdHeaderName, clientRequestId);
            return SqlClient;
        }
    }
}
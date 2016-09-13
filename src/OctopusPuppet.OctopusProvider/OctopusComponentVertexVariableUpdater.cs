﻿using System;
using System.Threading;
using Octopus.Client;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using OctopusPuppet.Deployer;
using OctopusPuppet.DeploymentPlanner;
using OctopusPuppet.Scheduler;

namespace OctopusPuppet.OctopusProvider
{
    public class OctopusComponentVertexVariableUpdater : IComponentVertexDeployer
    {
        private OctopusRepository _repository;

        public OctopusComponentVertexVariableUpdater(string octopusUrl, string octopusApiKey)
        {
            _repository = new OctopusRepository(new OctopusServerEndpoint(octopusUrl, octopusApiKey));
        }

        public ComponentVertexDeploymentResult Deploy(ComponentDeploymentVertex vertex, CancellationToken cancellationToken, IProgress<ComponentVertexDeploymentProgress> progress)
        {
            if (!vertex.Exists || vertex.VariableAction == VariableAction.Leave)
            {
                return new ComponentVertexDeploymentResult
                {
                    Status = ComponentVertexDeploymentStatus.Success
                };
            }

            var project = _repository.Projects.GetProjectByName(vertex.Name);
            var release = _repository.Projects.GetRelease(project.Id, vertex.Version);

            _repository.Releases.SnapshotVariables(release);

            return new ComponentVertexDeploymentResult
            {
                Status = ComponentVertexDeploymentStatus.Success,
                Description = "Updated"
            };
        }
    }
}
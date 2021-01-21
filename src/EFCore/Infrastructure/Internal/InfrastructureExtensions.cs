// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using CA = System.Diagnostics.CodeAnalysis;

#nullable enable

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static class InfrastructureExtensions
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        // TODO-NULLABLE: Do we really want this to accept (and return) null?
        [return: CA.NotNullIfNotNull("accessor")]
        public static TService? GetService<TService>([CanBeNull] IInfrastructure<IServiceProvider>? accessor)
            where TService : class
        {
            object? service = null;

            if (accessor != null)
            {
                var internalServiceProvider = accessor.Instance;

                service = internalServiceProvider.GetService(typeof(TService))
                    ?? internalServiceProvider.GetService<IDbContextOptions>()
                        ?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
                        ?.ApplicationServiceProvider
                        ?.GetService(typeof(TService));

                if (service == null)
                {
                    throw new InvalidOperationException(
                        CoreStrings.NoProviderConfiguredFailedToResolveService(typeof(TService).DisplayName()));
                }
            }

            return (TService?)service;
        }
    }
}

﻿using System;
using Exceptionless.Api.Tests.Authentication;
using Exceptionless.Api.Tests.Mail;
using Exceptionless.Core.Authentication;
using Exceptionless.Core.Mail;
using Foundatio.Extensions;
using Foundatio.Logging;
using Foundatio.Logging.Xunit;
using Foundatio.Utility;
using SimpleInjector;
using Xunit.Abstractions;

namespace Exceptionless.Api.Tests {
    public abstract class TestBase : TestWithLoggingBase, IDisposable {
        private Container _container;
        private bool _initialized;
        private readonly IDisposable _testSystemClock = TestSystemClock.Install();

        public TestBase(ITestOutputHelper output) : base(output) {
            Log.MinimumLevel = LogLevel.Information;
            Log.SetLogLevel<ScheduledTimer>(LogLevel.Warning);
        }

        public TService GetService<TService>() where TService : class {
            if (!_initialized)
                Initialize();

            return _container.GetService<TService>();
        }

        protected virtual void Initialize() {
            _container = GetDefaultContainer();
            _initialized = true;
        }

        protected virtual void RegisterServices(Container container) {
            Bootstrapper.RegisterServices(container, Log);

            container.Register<IMailer, NullMailer>();
            container.Register<IDomainLoginProvider, TestDomainLoginProvider>();
        }

        public Container GetDefaultContainer() {
            var container = AppBuilder.CreateContainer(Log, _logger, false);
            RegisterServices(container);
            return container;
        }

        public virtual void Dispose() {
            _testSystemClock.Dispose();
            _container?.Dispose();
        }
    }
}
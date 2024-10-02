# Resilience Support Library for .NET

## Introduction

The Resilience Support Library is a reusable.NET library designed to enhance application resilience and user experience by providing features for handling exceptions, retries, fallbacks, and feature toggling. This library integrates seamlessly with existing.NET infrastructure, providing developers with a robust and flexible toolkit to build resilient applications.

## Project Structure

1. Class Library Project: Developed in C# using .NET 6+.

2. Namespace: MyCompany. ResilienceLibrary.

## Dependencies

The following NuGet packages are required:

• Polly (for retry and fallback policies)

Serilog (for logging)

• Microsoft.Extensions. DependencyInjection (for IServiceProvider integration)

## Design Overview

1. Utility Classes:

• LoggingUtility: Wraps a Serilog Logger instance to provide logging methods.

• RetryUtility: Utilizes Polly's retry and fallback policies.

FeatureFlagsUtilit Manages feature flags and controls feature visibil↓


// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace Microsoft.Azure.Functions.Extensions.HelloWorld.Tests;

/// <summary>
/// Tests for HelloWorldTriggerAttribute validation.
/// </summary>
public class HelloWorldTriggerAttributeTests
{
    [Fact]
    public void Constructor_WithValidName_SetsNameProperty()
    {
        // Arrange & Act
        var attribute = new HelloWorldTriggerAttribute("TestUser");

        // Assert
        Assert.Equal("TestUser", attribute.Name);
        Assert.Null(attribute.MessagePrefix);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HelloWorldTriggerAttribute(null!));
    }

    [Fact]
    public void MessagePrefix_CanBeSet()
    {
        // Arrange
        var attribute = new HelloWorldTriggerAttribute("TestUser");

        // Act
        attribute.MessagePrefix = "👋";

        // Assert
        Assert.Equal("👋", attribute.MessagePrefix);
    }

    [Fact]
    public void MessagePrefix_DefaultsToNull()
    {
        // Arrange & Act
        var attribute = new HelloWorldTriggerAttribute("TestUser");

        // Assert
        Assert.Null(attribute.MessagePrefix);
    }
}

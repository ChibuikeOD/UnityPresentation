using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class SpeechTests
{
    private GameObject testObject;

    [SetUp]
    public void Setup()
    {
        // Create a GameObject for testing
        //Speech _myObject = gameObject.AddComponent<Speech>();
        // Awake() and Start() are executed here.
        //_myObject.SomeMethod();
        testObject = new GameObject();
        //testObject.AddComponent<Speech>();
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up after each test
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void LeftMethod_RotatesObjectToLeft()
    {
        // Arrange
        //Speech speechComponent = testObject.GetComponent<Speech>();

        // Act
        //speechComponent.leftMethod();

        // Assert
        Assert.AreEqual(Quaternion.Euler(0f, 90f, 0f), testObject.transform.rotation);
    }

    [Test]
    public void RightMethod_RotatesObjectToRight()
    {
        // Arrange
        //Speech speechComponent = testObject.GetComponent<Speech>();

        // Act
        //speechComponent.rightMethod();

        // Assert
        Assert.AreEqual(Quaternion.Euler(0f, -90f, 0f), testObject.transform.rotation);
    }

    [Test]
    public void UpMethod_MovesObjectUpward()
    {
        // Arrange
        //Speech speechComponent = testObject.GetComponent<Speech>();

        // Act
        //speechComponent.upMethod();

        // Assert
        Assert.AreEqual(new Vector3(0f, 0.2f, 0f), testObject.transform.position);
    }

    [Test]
    public void DownMethod_MovesObjectDownward()
    {
        // Arrange
        //Speech speechComponent = testObject.GetComponent<Speech>();

        // Act
        //speechComponent.downMethod();

        // Assert
        Assert.AreEqual(new Vector3(0f, -0.2f, 0f), testObject.transform.position);
    }
}
using System;
using NUnit.Framework;

namespace Task_2.UnitTests
{
    public class DoublyLinkedListTests
    {
        [Test]
        public void GettingTest()
        {
            int[] values = new int[6] { 1, 2, 3, 4, 5, 6 };
            DoublyLinkedList<int> testList = new DoublyLinkedList<int>();
            for (int i = 0; i < 6; i++)
            {
                testList.Add(values[i]);
            }
            int value = testList.GetByIndex(2);
            Assert.AreEqual(value, values[2]);
            value = testList.GetByIndex(-2);
            Assert.AreEqual(value, values[4]);
            value = testList.GetByIndex(1);
            Assert.AreEqual(value, values[1]);
            try
            {
                value = testList.GetByIndex(12);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                string exMessage = "Specified argument was out of the range of valid values. (Parameter 'Index was out of range')";
                Assert.AreEqual(exMessage, ex.Message);
            }
        }

        [Test]
        public void AddingTest()
        {
            int[] values = new int[5] { 1, 2, 3, 4, 5 };
            DoublyLinkedList<int> testList = new DoublyLinkedList<int>();
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(testList.Add(values[i]), values[i]);
            }
            Assert.AreEqual(values.Length, testList.Length);
        }

        [Test]
        public void RemovingByIndexTest()
        {
            int[] values = new int[6] { 1, 2, 3, 4, 5, 6 };
            DoublyLinkedList<int> testList = new DoublyLinkedList<int>();
            for (int i = 0; i < 6; i++)
            {
                testList.Add(values[i]);
            }
            testList.DeleteByIndex(2);
            testList.DeleteByIndex(-2);
            Assert.AreEqual(testList.GetByIndex(0), values[0]);
            Assert.AreEqual(testList.GetByIndex(1), values[1]);
            Assert.AreEqual(testList.GetByIndex(2), values[3]);
            Assert.AreEqual(testList.GetByIndex(3), values[5]);
            Assert.AreEqual(testList.Length, 4);
            testList.DeleteByIndex(3);
            testList.DeleteByIndex(2);
            Assert.AreEqual(testList.GetByIndex(0), values[0]);
            Assert.AreEqual(testList.GetByIndex(1), values[1]);
            Assert.AreEqual(testList.Length, 2);
            try
            {
                testList.DeleteByIndex(2);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                string exMessage = "Specified argument was out of the range of valid values. (Parameter 'Index was out of range')";
                Assert.AreEqual(exMessage, ex.Message);
            }
            testList.DeleteByIndex(1);
            Assert.AreEqual(testList.GetByIndex(0), values[0]);
            Assert.AreEqual(testList.Length, 1);
            testList.DeleteByIndex(0);
            Assert.AreEqual(testList.Length, 0);
            for (int i = 0; i < 3; i++)
            {
                testList.Add(values[i]);
            }
            testList.DeleteByIndex(0);
            Assert.AreEqual(testList.GetByIndex(0), values[1]);
            Assert.AreEqual(testList.GetByIndex(1), values[2]);
            Assert.AreEqual(testList.Length, 2);
        }

        [Test]
        public void RemovingByValueTest()
        {
            int[] values = new int[6] { 1, 2, 3, 4, 5, 6 };
            DoublyLinkedList<int> testList = new DoublyLinkedList<int>();
            for (int i = 0; i < 6; i++)
            {
                testList.Add(values[i]);
            }
            Assert.AreEqual(testList.DeleteByValue(2), 0);
            Assert.AreEqual(testList.DeleteByValue(2), 1);
            Assert.AreEqual(testList.GetByIndex(0), values[0]);
            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(testList.GetByIndex(i), values[i + 1]);
            }
            Assert.AreEqual(testList.Length, 5);
        }

        [Test]
        public void ListConstructorTest()
        {
            DoublyLinkedList<int> testList = new DoublyLinkedList<int>();
            Assert.AreEqual(testList.Beginning, null);
            Assert.AreEqual(testList.Ending, null);
            Assert.AreEqual(testList.Length, 0);
        }

        [Test]
        public void NodeConstructorTest()
        {
            Node<int> testPreviousNode = new Node<int>(5);
            Node<int> testNextNode = new Node<int>(6);
            int testValue = 10;
            Node<int> testNode = new Node<int>(testValue, testPreviousNode, testNextNode);
            Assert.AreEqual(testNode.Value, testValue);
            Assert.AreEqual(testNode.Previous, testPreviousNode);
            Assert.AreEqual(testNode.Next, testNextNode);
        }

        [Test]
        public void SetNodesTest()
        {
            Node<int> testPreviousNode = new Node<int>(5);
            Node<int> testNextNode = new Node<int>(6);
            int testValue = 10;
            Node<int> testNode = new Node<int>(testValue);
            testNode.SetNodes(testPreviousNode, testNextNode);
            Assert.AreEqual(testNode.Value, testValue);
            Assert.AreEqual(testNode.Previous, testPreviousNode);
            Assert.AreEqual(testNode.Next, testNextNode);
        }
    }
}
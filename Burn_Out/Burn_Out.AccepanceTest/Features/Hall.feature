Feature: Hall management
  In order to manage rooms in the system
  As an administrator
  I want to be able to add and see halls

  Scenario: Administrator adds a new hall
    Given I have an empty database
    When I create a hall with name "Sala A" and capacity 100
    Then the hall "Sala A" with capacity 100 should exist

Feature: Hall reservation
  As a user
  I want to reserve a hall
  So that I can schedule events

  Background:
    Given the application is running
    And a user with email "user@example.com" and password "P@ssw0rd!" exists

  Scenario: Reserve an available hall succeeds
    Given a hall exists with id 1 named "Main Hall" and capacity 100
    When I reserve hall 1 from "2026-02-01T10:00:00Z" to "2026-02-01T12:00:00Z" as "user@example.com"
    Then the reservation should succeed
    And hall 1 should be unavailable

  Scenario: Overlapping reservation fails
    Given a hall exists with id 2 named "Side Hall" and capacity 50
    And I reserve hall 2 from "2026-02-01T10:00:00Z" to "2026-02-01T12:00:00Z" as "user@example.com"
    When I reserve hall 2 from "2026-02-01T11:00:00Z" to "2026-02-01T13:00:00Z" as "user@example.com"
    Then the reservation should fail
    And hall 2 should be unavailable
